using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace TravelAgencyManagementSystem.Data;

internal sealed class Account
{
    public ObjectId Id { get; set; }
    public string FullName { get; set; } = "";
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "Administrator";
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

[BsonIgnoreExtraElements]
internal sealed class Trip
{
    public ObjectId Id { get; set; }
    public string Destination { get; set; } = "";
    public DateTime DepartureDate { get; set; }
    public int Duration { get; set; }
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; set; }
    public int Capacity { get; set; }
    public int AvailableSeats { get; set; }
    [BsonIgnoreIfNull]
    public SeatChange? PendingSeats { get; set; }
    public string Status { get; set; } = "Open";
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

internal sealed class Client
{
    public ObjectId Id { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Nationality { get; set; } = "";
    public string PassportOrId { get; set; } = "";
    public string Status { get; set; } = "Active";
    public string Address { get; set; } = "";
    public string Notes { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

internal sealed partial class AgencyStore
{
    private const int PasswordIterations = 600_000;
    private static readonly ObjectId BootstrapAdminId = ObjectId.Parse("000000000000000000000001");
    private static readonly Lazy<AgencyStore> Instance = new(() => new AgencyStore());
    public static AgencyStore Current => Instance.Value;
    private readonly IMongoDatabase database;
    private IMongoCollection<Account> Users => database.GetCollection<Account>("Users");
    private IMongoCollection<Client> Clients => database.GetCollection<Client>("Clients");
    private IMongoCollection<Trip> Trips => database.GetCollection<Trip>("Trips");
    private IMongoCollection<Booking> Bookings => database.GetCollection<Booking>("Bookings");
    private IMongoCollection<Payment> Payments => database.GetCollection<Payment>("Payments");
    private const string Connection = "mongodb://localhost:27017";

    private AgencyStore()
    {
        var settings = MongoClientSettings.FromConnectionString(Connection);
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(4);
        settings.SocketTimeout = TimeSpan.FromSeconds(10);
        database = new MongoClient(settings).GetDatabase("TravelAgencyDB");
    }

    public async Task InitializeAsync()
    {
        await database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
        await IndexAsync(Users, "Username", true);
        await IndexAsync(Users, "Email", true);
        await IndexAsync(Clients, "Email", true);
        await IndexAsync(Trips, "Destination");
        await IndexAsync(Bookings, "BookingNumber", true);
        await IndexAsync(Bookings, "ClientId");
        await IndexAsync(Bookings, "TripId");
        await IndexAsync(Payments, "PaymentNumber", true);
        await IndexAsync(Payments, "BookingId");
        await RecoverReservationsAsync();
    }

    private static Task<string> IndexAsync<T>(IMongoCollection<T> collection, string field, bool unique = false) =>
        collection.Indexes.CreateOneAsync(new CreateIndexModel<T>(
            Builders<T>.IndexKeys.Ascending(field), new CreateIndexOptions { Unique = unique }));

    public Task<bool> HasUsersAsync() => Users.Find(FilterDefinition<Account>.Empty).AnyAsync();

    public async Task CreateFirstAdministratorAsync(string username, string email, string password)
    {
        username = NormalizeUsername(username);
        email = ValidateEmail(email);
        if (await HasUsersAsync())
            throw new InvalidOperationException("An account already exists. Sign in with your existing credentials.");
        var passwordHash = await HashPasswordAsync(password);
        await Users.InsertOneAsync(new Account
        {
            // A fixed bootstrap ID prevents simultaneous first-run attempts from creating multiple administrators.
            Id = BootstrapAdminId, FullName = "Agency Administrator",
            Username = username, Email = email, PasswordHash = passwordHash
        });
    }

    public async Task<Account> SignInAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || password.Length > 256)
            throw new ArgumentException("Enter your username and password.");
        var normalized = username.Trim().ToLowerInvariant();
        var user = await Users.Find(x => x.Username == normalized).FirstOrDefaultAsync();
        var parts = user?.PasswordHash.Split('.');
        if (parts is null || parts.Length != 2)
            throw new InvalidOperationException("Incorrect username or password.");
        var expected = Convert.FromBase64String(parts[1]);
        var salt = Convert.FromBase64String(parts[0]);
        var actual = await Task.Run(() => Rfc2898DeriveBytes.Pbkdf2(password, salt, PasswordIterations, HashAlgorithmName.SHA256, 32));
        if (!CryptographicOperations.FixedTimeEquals(actual, expected))
            throw new InvalidOperationException("Incorrect username or password.");
        if (user!.Status != "Active")
            throw new InvalidOperationException("This account is inactive.");
        user.PasswordHash = "";
        return user;
    }

    public async Task<(long Trips, long Clients)> CountRecordsAsync()
    {
        var trips = Trips.CountDocumentsAsync(FilterDefinition<Trip>.Empty);
        var clients = Clients.CountDocumentsAsync(FilterDefinition<Client>.Empty);
        await Task.WhenAll(trips, clients);
        return (await trips, await clients);
    }

    public async Task<List<Trip>> LoadTripsAsync()
    {
        await RecoverSeatChangesAsync();
        return await Trips.Find(FilterDefinition<Trip>.Empty).SortBy(x => x.DepartureDate).ToListAsync();
    }
    public Task<List<Client>> LoadClientsAsync() => Clients.Find(FilterDefinition<Client>.Empty).SortBy(x => x.FullName).ToListAsync();

    private async Task RequireManagementAsync(ObjectId actor)
    {
        if (!await Users.Find(x => x.Id == actor && x.Status == "Active" &&
            (x.Role == "Administrator" || x.Role == "Agent")).AnyAsync())
            throw new InvalidOperationException("Your account cannot manage trips or clients.");
    }

    public async Task SaveTripAsync(ObjectId actor, Trip trip)
    {
        await RequireManagementAsync(actor);
        trip.Destination = trip.Destination.Trim();
        if (trip.Destination.Length == 0 || trip.Duration <= 0 || trip.Capacity <= 0 ||
            trip.Price < 0 || decimal.Round(trip.Price, 2) != trip.Price ||
            trip.Status is not ("Open" or "Full" or "Cancelled"))
            throw new ArgumentException("Enter a destination, positive duration and capacity, and a valid price and status.");
        if (trip.Id == ObjectId.Empty)
        {
            trip.AvailableSeats = trip.Capacity;
            trip.Status = trip.Status == "Cancelled" ? "Cancelled" : "Open";
            await Trips.InsertOneAsync(trip);
            return;
        }
        await RecoverSeatChangesAsync();
        var existing = await Trips.Find(x => x.Id == trip.Id).FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("The trip no longer exists.");
        if (existing.UpdatedAt != trip.UpdatedAt)
            throw new InvalidOperationException("The trip changed. Refresh and retry.");
        var booked = existing.Capacity - existing.AvailableSeats;
        if (trip.Status == "Cancelled" && existing.Status != "Cancelled" && booked > 0)
            throw new ArgumentException("Cancel the active bookings before cancelling this trip.");
        if (trip.Capacity < booked)
            throw new ArgumentException("Capacity cannot be lower than the number of booked seats.");
        trip.UpdatedAt = DateTime.UtcNow;
        trip.CreatedAt = existing.CreatedAt;
        trip.AvailableSeats = trip.Capacity - booked;
        if (trip.Status != "Cancelled")
            trip.Status = trip.AvailableSeats == 0 ? "Full" : "Open";
        var result = await Trips.UpdateOneAsync(x => x.Id == trip.Id && x.UpdatedAt == existing.UpdatedAt &&
            x.AvailableSeats == existing.AvailableSeats && x.PendingSeats == null, Builders<Trip>.Update
                .Set(x => x.Destination, trip.Destination).Set(x => x.DepartureDate, trip.DepartureDate)
                .Set(x => x.Duration, trip.Duration).Set(x => x.Price, trip.Price).Set(x => x.Capacity, trip.Capacity)
                .Set(x => x.AvailableSeats, trip.AvailableSeats).Set(x => x.Status, trip.Status)
                .Set(x => x.Description, trip.Description).Set(x => x.UpdatedAt, trip.UpdatedAt));
        if (result.MatchedCount == 0)
            throw new InvalidOperationException("The trip changed during the update. Reopen the trip and retry.");
    }

    public async Task ArchiveTripAsync(ObjectId actor, ObjectId id)
    {
        await RequireManagementAsync(actor);
        await RecoverSeatChangesAsync();
        if (await Bookings.Find(x => x.TripId == id).AnyAsync())
            throw new InvalidOperationException("This trip has bookings and cannot be archived.");
        var result = await Trips.UpdateOneAsync(x => x.Id == id && x.PendingSeats == null && x.AvailableSeats == x.Capacity, Builders<Trip>.Update
            .Set(x => x.Status, "Cancelled").Set(x => x.UpdatedAt, DateTime.UtcNow));
        if (result.MatchedCount == 0)
            throw new InvalidOperationException("The trip no longer exists or has an active reservation.");
    }

    public async Task SaveClientAsync(ObjectId actor, Client client)
    {
        await RequireManagementAsync(actor);
        client.FullName = client.FullName.Trim();
        client.Phone = client.Phone.Trim();
        if (client.FullName.Length == 0 || string.IsNullOrWhiteSpace(client.Nationality) ||
            client.Status is not ("Active" or "Inactive") ||
            !Regex.IsMatch(client.Phone, @"^\+?[0-9 ()-]+$") ||
            client.Phone.Count(char.IsAsciiDigit) is < 7 or > 15)
            throw new ArgumentException("Enter a name, nationality and phone number with 7–15 digits.");
        client.Email = ValidateEmail(client.Email);
        var version = client.UpdatedAt;
        client.UpdatedAt = DateTime.UtcNow;
        if (client.Id == ObjectId.Empty)
            await Clients.InsertOneAsync(client);
        else if ((await Clients.ReplaceOneAsync(x => x.Id == client.Id && x.UpdatedAt == version, client)).MatchedCount == 0)
            throw new InvalidOperationException("The client changed or no longer exists. Refresh and retry.");
    }

    public async Task DeactivateClientAsync(ObjectId actor, ObjectId id)
    {
        await RequireManagementAsync(actor);
        var result = await Clients.UpdateOneAsync(x => x.Id == id, Builders<Client>.Update
            .Set(x => x.Status, "Inactive").Set(x => x.UpdatedAt, DateTime.UtcNow));
        if (result.MatchedCount == 0)
            throw new InvalidOperationException("The client no longer exists.");
    }

    private static string ValidateEmail(string input)
    {
        var email = input.Trim().ToLowerInvariant();
        if (!MailAddress.TryCreate(email, out var parsed) || parsed.Address != email)
            throw new ArgumentException("Enter a valid email address.");
        return email;
    }
}


