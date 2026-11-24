using TravelAgencyManagementSystem.Data;
using TravelAgencyManagementSystem.Forms;

namespace TravelAgencyManagementSystem.UserControls;

public partial class TripsControl : UserControl
{
    private Account? account;
    private List<Trip> trips = [];
    private Trip? Selected => trips.FirstOrDefault(trip => trip.Id.ToString() == recordsPanel.SelectedRecord?[0]);

    public TripsControl()
    {
        InitializeComponent();
        recordsPanel.Configure("Explore your trips", "Organize destinations, departures, and travel experiences.", "+  Add trip",
            ["Trip ID", "Destination", "Departure", "Duration", "Price (EGP)", "Booked / Total", "Status"], []);
        recordsPanel.SetColumnWidths(130, 135, 105, 75, 90, 100, 80);
        recordsPanel.SetGroupFilter("Destination", 1);
        recordsPanel.SetStatusFilter("Open", "Full", "Cancelled");
        recordsPanel.AddRequested += async (_, _) => await ShowTripAsync("Add trip", null);
        recordsPanel.EditRequested += async (_, _) => await ShowTripAsync("Edit trip", Selected);
        recordsPanel.DetailsRequested += async (_, _) => await ShowTripAsync("Trip details", Selected, true);
        recordsPanel.DeleteRequested += async (_, _) =>
        {
            if (account is not { } actor || Selected is not { } trip ||
                MessageBox.Show(this, $"Mark the trip to {trip.Destination} as cancelled?", "Cancel trip",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            if (await UiOperation.RunAsync(this, () => AgencyStore.Current.ArchiveTripAsync(actor.Id, trip.Id)))
                await RefreshAsync(actor);
        };
    }

    internal Task<bool> RefreshAsync(Account actor) => UiOperation.RunAsync(this, async () =>
    {
        account = actor;
        recordsPanel.SetWriteAccess(actor.Role is "Administrator" or "Agent");
        trips = await AgencyStore.Current.LoadTripsAsync();
        if (!IsDisposed)
            recordsPanel.ReplaceRows(trips.Select(trip => new[]
            {
                trip.Id.ToString(), trip.Destination, trip.DepartureDate.ToString("dd MMM yyyy"),
                $"{trip.Duration} days", trip.Price.ToString("N2"),
                $"{trip.Capacity - trip.AvailableSeats} / {trip.Capacity}", trip.Status
            }).ToArray());
    });

    private async Task ShowTripAsync(string title, Trip? trip, bool readOnly = false)
    {
        if (account is not { } actor || (trip is null && title != "Add trip"))
            return;
        using var dialog = new TripDialog(title, trip, readOnly);
        dialog.SaveAsync = value => AgencyStore.Current.SaveTripAsync(actor.Id, value);
        if (dialog.ShowDialog(this) == DialogResult.OK)
            await RefreshAsync(actor);
    }
}
