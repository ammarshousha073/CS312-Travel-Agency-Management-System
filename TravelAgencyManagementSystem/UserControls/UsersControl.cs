using TravelAgencyManagementSystem.Data;
using TravelAgencyManagementSystem.Forms;

namespace TravelAgencyManagementSystem.UserControls;

public partial class UsersControl : UserControl
{
    private Account? account;
    private List<UserProfile> users = [];
    private UserProfile? Selected => users.Find(user => user.Id.ToString() == recordsPanel.SelectedRecord?[0]);

    public UsersControl()
    {
        InitializeComponent();
        recordsPanel.Configure("Your agency team", "View team members, their roles, and account status.", "+  Add user",
            ["User ID", "Full name", "Username", "Email", "Role", "Status"], []);
        recordsPanel.SetColumnWidths(115, 125, 120, 165, 110, 80);
        recordsPanel.SetGroupFilter("Role", 4);
        recordsPanel.SetStatusFilter("Active", "Inactive");
        recordsPanel.SetActions(true, false);
        recordsPanel.SetWriteAccess(false);
        recordsPanel.AddRequested += async (_, _) => await ShowEditorAsync(null);
        recordsPanel.EditRequested += async (_, _) =>
        {
            if (Selected is { } user)
                await ShowEditorAsync(user);
        };
        recordsPanel.DetailsRequested += (_, _) => ShowDetails();
    }

    internal Task<bool> RefreshAsync(Account actor) => UiOperation.RunAsync(this, async () =>
    {
        account = null;
        recordsPanel.SetWriteAccess(false);
        if (actor.Role != "Administrator")
        {
            ClearUsers();
            return;
        }

        List<UserProfile> profiles;
        try
        {
            profiles = await AgencyStore.Current.LoadUsersAsync(actor.Id);
        }
        catch
        {
            if (!IsDisposed)
                ClearUsers();
            throw;
        }
        if (IsDisposed)
            return;

        account = actor;
        users = profiles;
        recordsPanel.SetActions(true, false);
        recordsPanel.SetWriteAccess(true);
        recordsPanel.ReplaceRows(users.Select(user => new[]
        {
            user.Id.ToString(), user.FullName, user.Username, user.Email, user.Role, user.Status
        }).ToArray());
    });

    private void ClearUsers()
    {
        users = [];
        recordsPanel.ReplaceRows([]);
        recordsPanel.SetNotice("User records are unavailable.");
    }

    private async Task ShowEditorAsync(UserProfile? user)
    {
        if (account is not { } actor)
            return;
        var fullName = RecordDialog.TextInput(user?.FullName ?? "");
        var username = RecordDialog.TextInput(user?.Username ?? "");
        var email = RecordDialog.TextInput(user?.Email ?? "");
        var role = RecordDialog.ChoiceInput(user?.Role ?? "Agent", AgencyStore.Roles.ToArray());
        var status = RecordDialog.ChoiceInput(user?.Status ?? "Active", "Active", "Inactive");
        var password = RecordDialog.TextInput();
        password.UseSystemPasswordChar = true;
        password.MaxLength = 256;
        if (user?.Id == actor.Id)
            role.Enabled = status.Enabled = false;

        using var dialog = new RecordDialog(user is null ? "Add user" : "Edit user",
            user?.Username ?? "New team member", false,
            ("Full name", fullName), ("Username", username), ("Email", email), ("Role", role), ("Status", status),
            (user is null ? "Password (12+ characters)" : "New password (optional)", password));

        dialog.SaveAsync = () => AgencyStore.Current.SaveUserAsync(actor.Id,
            new UserProfile(user?.Id ?? default, fullName.Text, username.Text, email.Text, role.Text,
                status.Text, user?.CreatedAt ?? default, user?.UpdatedAt ?? default), password.Text);
        if (dialog.ShowDialog(this) == DialogResult.OK)
            await RefreshAsync(actor);
    }

    private void ShowDetails()
    {
        if (Selected is not { } user)
            return;
        using var dialog = new RecordDialog("User details", user.Id.ToString(), true,
            ("Full name", RecordDialog.TextInput(user.FullName)), ("Username", RecordDialog.TextInput(user.Username)),
            ("Email", RecordDialog.TextInput(user.Email)), ("Role", RecordDialog.TextInput(user.Role)),
            ("Status", RecordDialog.TextInput(user.Status)),
            ("Created", RecordDialog.TextInput(user.CreatedAt.ToLocalTime().ToString("g"))),
            ("Last updated", RecordDialog.TextInput(user.UpdatedAt.ToLocalTime().ToString("g"))));
        dialog.ShowDialog(this);
    }
}
