using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace TravelAgencyManagementSystem.Data;

internal sealed record UserProfile(ObjectId Id, string FullName, string Username, string Email,
    string Role, string Status, DateTime CreatedAt, DateTime UpdatedAt);

internal sealed partial class AgencyStore
{
    public static IReadOnlyList<string> Roles { get; } =
        Array.AsReadOnly(new[] { "Administrator", "Agent", "Accountant", "Viewer" });

    private async Task RequireAdministratorAsync(ObjectId actor)
    {
        if (!await Users.Find(x => x.Id == actor && x.Status == "Active" && x.Role == "Administrator").AnyAsync())
            throw new InvalidOperationException("An active administrator account is required to manage users.");
    }

    public async Task<List<UserProfile>> LoadUsersAsync(ObjectId actor)
    {
        await RequireAdministratorAsync(actor);
        return await Users.Find(FilterDefinition<Account>.Empty).SortBy(x => x.FullName)
            .Project(x => new UserProfile(x.Id, x.FullName, x.Username, x.Email, x.Role, x.Status,
                x.CreatedAt, x.UpdatedAt)).ToListAsync();
    }

    public async Task SaveUserAsync(ObjectId actor, UserProfile profile, string password)
    {
        await RequireAdministratorAsync(actor);
        var username = NormalizeUsername(profile.Username);
        var email = ValidateEmail(profile.Email);
        var fullName = profile.FullName.Trim();
        if (fullName.Length == 0 || !Roles.Contains(profile.Role) || profile.Status is not ("Active" or "Inactive"))
            throw new ArgumentException("Enter a full name and select a valid role and status.");

        if (profile.Id == ObjectId.Empty)
        {
            var hash = await HashPasswordAsync(password);
            await RequireAdministratorAsync(actor);
            await Users.InsertOneAsync(new Account
            {
                FullName = fullName, Username = username, Email = email, PasswordHash = hash,
                Role = profile.Role, Status = profile.Status
            });
            return;
        }

        var current = await Users.Find(x => x.Id == profile.Id).Project(x => new UserProfile(
            x.Id, x.FullName, x.Username, x.Email, x.Role, x.Status, x.CreatedAt, x.UpdatedAt)).FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("The user no longer exists.");
        if (current.UpdatedAt != profile.UpdatedAt)
            throw new InvalidOperationException("This account changed. Reopen it and retry.");

        if (current.Role == "Administrator" && current.Status == "Active" &&
            (profile.Role != "Administrator" || profile.Status != "Active"))
        {
            if (current.Id == actor)
                throw new InvalidOperationException("You cannot remove administrator access from your current account.");
            // The bootstrap account has the lowest ID. For an older database, protect its lowest active administrator.
            // Every permission-reducing update leaves this administrator active.
            var protectedId = await Users.Find(x => x.Status == "Active" && x.Role == "Administrator")
                .SortBy(x => x.Id).Project(x => x.Id).FirstOrDefaultAsync();
            if (current.Id == protectedId)
                throw new InvalidOperationException("The primary administrator must remain active.");
        }

        var updatedAt = DateTime.UtcNow;
        if (updatedAt < current.UpdatedAt.AddMilliseconds(1))
            updatedAt = current.UpdatedAt.AddMilliseconds(1);
        var update = Builders<Account>.Update.Set(x => x.FullName, fullName).Set(x => x.Username, username)
            .Set(x => x.Email, email).Set(x => x.Role, profile.Role).Set(x => x.Status, profile.Status)
            .Set(x => x.UpdatedAt, updatedAt);
        if (password.Length > 0)
            update = update.Set(x => x.PasswordHash, await HashPasswordAsync(password));

        await RequireAdministratorAsync(actor);
        var result = await Users.UpdateOneAsync(x => x.Id == profile.Id && x.UpdatedAt == current.UpdatedAt, update);
        if (result.MatchedCount == 0)
            throw new InvalidOperationException("This account changed. Reopen it and retry.");
    }

    private static string NormalizeUsername(string input)
    {
        var username = input.Trim().ToLowerInvariant();
        if (!Regex.IsMatch(username, @"^[a-z0-9_.-]{3,32}$"))
            throw new ArgumentException("Use a username of 3–32 letters, digits, periods, hyphens, or underscores.");
        return username;
    }

    private static async Task<string> HashPasswordAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length is < 12 or > 256)
            throw new ArgumentException("Use a password of 12–256 characters.");
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = await Task.Run(() => Rfc2898DeriveBytes.Pbkdf2(password, salt, PasswordIterations, HashAlgorithmName.SHA256, 32));
        return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
    }
}
