using TravelAgencyManagementSystem.Data;
using TravelAgencyManagementSystem.Forms;

namespace TravelAgencyManagementSystem.UserControls;

public partial class ClientsControl : UserControl
{
    private Account? account;
    private List<Client> clients = [];
    private Client? Selected => clients.FirstOrDefault(client => client.Id.ToString() == recordsPanel.SelectedRecord?[0]);

    public ClientsControl()
    {
        InitializeComponent();
        recordsPanel.Configure("Your travelers", "Keep client information clear, organized, and easy to find.", "+  Add client",
            ["Client ID", "Client name", "Phone", "Email", "Nationality", "Status"], []);
        recordsPanel.SetColumnWidths(130, 115, 110, 175, 90, 70);
        recordsPanel.SetGroupFilter("Nationality", 4);
        recordsPanel.SetStatusFilter("Active", "Inactive");
        recordsPanel.AddRequested += async (_, _) => await ShowClientAsync("Add client", null);
        recordsPanel.EditRequested += async (_, _) => await ShowClientAsync("Edit client", Selected);
        recordsPanel.DetailsRequested += async (_, _) => await ShowClientAsync("Client details", Selected, true);
        recordsPanel.DeleteRequested += async (_, _) =>
        {
            if (account is not { } actor || Selected is not { } client ||
                MessageBox.Show(this, $"Deactivate {client.FullName}?", "Deactivate client",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            if (await UiOperation.RunAsync(this, () => AgencyStore.Current.DeactivateClientAsync(actor.Id, client.Id)))
                await RefreshAsync(actor);
        };
    }

    internal Task<bool> RefreshAsync(Account actor) => UiOperation.RunAsync(this, async () =>
    {
        account = actor;
        recordsPanel.SetWriteAccess(actor.Role is "Administrator" or "Agent");
        clients = await AgencyStore.Current.LoadClientsAsync();
        if (!IsDisposed)
            recordsPanel.ReplaceRows(clients.Select(client => new[]
            {
                client.Id.ToString(), client.FullName, client.Phone, client.Email, client.Nationality, client.Status
            }).ToArray());
    });

    private async Task ShowClientAsync(string title, Client? client, bool readOnly = false)
    {
        if (account is not { } actor || (client is null && title != "Add client"))
            return;
        var name = RecordDialog.TextInput(client?.FullName ?? "");
        var phone = RecordDialog.TextInput(client?.Phone ?? "");
        var email = RecordDialog.TextInput(client?.Email ?? "");
        var nationality = RecordDialog.TextInput(client?.Nationality ?? "Egyptian");
        var document = RecordDialog.TextInput(client?.PassportOrId ?? "");
        var status = RecordDialog.ChoiceInput(client?.Status ?? "Active", "Active", "Inactive");
        var address = RecordDialog.TextInput(client?.Address ?? "");
        var notes = RecordDialog.TextInput(client?.Notes ?? "");
        using var dialog = new RecordDialog(title, client?.Id.ToString() ?? "New client", readOnly,
            ("Full name", name), ("Phone number", phone), ("Email address", email), ("Nationality", nationality),
            ("Passport / ID", document), ("Status", status), ("Address", address), ("Notes", notes));
        dialog.SaveAsync = () => AgencyStore.Current.SaveClientAsync(actor.Id, new Client
        {
            Id = client?.Id ?? default, CreatedAt = client?.CreatedAt ?? DateTime.UtcNow,
            UpdatedAt = client?.UpdatedAt ?? DateTime.UtcNow, FullName = name.Text, Phone = phone.Text,
            Email = email.Text, Nationality = nationality.Text.Trim(), PassportOrId = document.Text.Trim(),
            Status = status.Text, Address = address.Text.Trim(), Notes = notes.Text.Trim()
        });
        if (dialog.ShowDialog(this) == DialogResult.OK)
            await RefreshAsync(actor);
    }
}
