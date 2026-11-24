using MongoDB.Bson;
using TravelAgencyManagementSystem.Data;
using TravelAgencyManagementSystem.Forms;

namespace TravelAgencyManagementSystem.UserControls;

public partial class BookingsControl : UserControl
{
    private Account? account;
    private List<BookingRow> rows = [];
    private BookingRow? Selected => rows.Find(row => row.Booking.BookingNumber == recordsPanel.SelectedRecord?[0]);
    internal event EventHandler? ReservationsChanged;

    public BookingsControl()
    {
        InitializeComponent();
        recordsPanel.Configure("Bookings at a glance", "Review reservations and connect travelers with their next trip.", "+  Create booking",
            ["Booking ID", "Client", "Destination", "Booking date", "Travelers", "Total (EGP)", "Paid (EGP)", "Due (EGP)", "Status"], []);
        recordsPanel.SetColumnWidths(155, 105, 105, 90, 65, 85, 85, 85, 75);
        recordsPanel.SetGroupFilter("Destination", 2);
        recordsPanel.SetStatusFilter("Confirmed", "Pending", "Cancelled");
        recordsPanel.SetActions(false, true);
        recordsPanel.SetDeleteText("Cancel booking");
        recordsPanel.AddRequested += async (_, _) => await CreateAsync();
        recordsPanel.DeleteRequested += async (_, _) => await CancelAsync();
        recordsPanel.DetailsRequested += (_, _) => ShowDetails();
    }

    internal Task<bool> RefreshAsync(Account actor) => UiOperation.RunAsync(this, async () =>
    {
        account = actor;
        recordsPanel.SetWriteAccess(actor.Role is "Administrator" or "Agent");
        var snapshot = await AgencyStore.Current.LoadReservationsAsync();
        if (IsDisposed)
            return;
        rows = snapshot.Bookings;
        recordsPanel.ReplaceRows(rows.Select(row => new[]
        {
            row.Booking.BookingNumber, row.Client, row.Destination, row.Booking.BookingDate.ToString("dd MMM yyyy"),
            row.Booking.Travelers.ToString(), row.Booking.TotalAmount.ToString("N2"), row.Booking.PaidAmount.ToString("N2"),
            row.Booking.RemainingAmount.ToString("N2"), row.Booking.Status
        }).ToArray());
    });

    private async Task CreateAsync()
    {
        if (account is not { } actor)
            return;
        List<Client> clients = [];
        List<Trip> trips = [];
        if (!await UiOperation.RunAsync(this, async () =>
        {
            var clientTask = AgencyStore.Current.LoadClientsAsync();
            var tripTask = AgencyStore.Current.LoadTripsAsync();
            await Task.WhenAll(clientTask, tripTask);
            clients = (await clientTask).Where(client => client.Status == "Active").ToList();
            trips = (await tripTask).Where(trip => trip.Status == "Open" && trip.AvailableSeats > 0 &&
                trip.DepartureDate.Date >= DateTime.Today).ToList();
        }) || IsDisposed)
            return;

        if (clients.Count == 0 || trips.Count == 0)
        {
            MessageBox.Show(this, "Create an active client and an upcoming trip with available seats first.", "Create booking");
            return;
        }

        var client = RecordDialog.ChoiceInput("", clients.Select(item => $"{item.FullName} · {item.Email}").ToArray());
        var trip = RecordDialog.ChoiceInput("", trips.Select(item =>
            $"{item.Destination} · {item.DepartureDate:dd MMM yyyy} · {item.AvailableSeats} seats").ToArray());
        var date = new DateTimePicker { Format = DateTimePickerFormat.Short };
        var travelers = new NumericUpDown { Minimum = 1, Maximum = int.MaxValue, Value = 1 };
        var requests = RecordDialog.TextInput();
        var total = RecordDialog.TextInput();
        total.ReadOnly = true;
        void UpdateTotal() => total.Text = (trips[trip.SelectedIndex].Price * travelers.Value).ToString("N2");
        trip.SelectedIndexChanged += (_, _) => UpdateTotal();
        travelers.ValueChanged += (_, _) => UpdateTotal();
        UpdateTotal();

        var requestId = ObjectId.GenerateNewId();
        using var dialog = new RecordDialog("Create booking", "Select a client and departure", false,
            ("Client", client), ("Trip", trip), ("Booking date", date), ("Travelers", travelers),
            ("Estimated total (EGP)", total), ("Special requests", requests));
        dialog.SaveAsync = () => AgencyStore.Current.CreateBookingAsync(actor.Id, requestId,
            clients[client.SelectedIndex].Id, trips[trip.SelectedIndex].Id, date.Value, (int)travelers.Value, requests.Text);
        if (dialog.ShowDialog(this) == DialogResult.OK)
            await AfterWriteAsync(actor);
    }

    private async Task CancelAsync()
    {
        if (account is not { } actor || Selected is not { } row || row.Booking.Status == "Cancelled")
            return;
        if (MessageBox.Show(this, $"Cancel {row.Booking.BookingNumber} and release its seats?",
            "Cancel booking", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        if (await UiOperation.RunAsync(this, () => AgencyStore.Current.CancelBookingAsync(actor.Id, row.Booking.Id)))
            await AfterWriteAsync(actor);
    }

    private async Task AfterWriteAsync(Account actor)
    {
        await RefreshAsync(actor);
        if (!IsDisposed)
            ReservationsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ShowDetails()
    {
        if (Selected is not { } row)
            return;
        var booking = row.Booking;
        using var dialog = new RecordDialog("Booking details", booking.BookingNumber, true,
            ("Client", RecordDialog.TextInput(row.Client)), ("Destination", RecordDialog.TextInput(row.Destination)),
            ("Booking date", RecordDialog.TextInput(booking.BookingDate.ToString("dd MMM yyyy"))),
            ("Departure", RecordDialog.TextInput(row.Departure?.ToString("dd MMM yyyy") ?? "Unknown")),
            ("Travelers", RecordDialog.TextInput(booking.Travelers.ToString())),
            ("Booking status", RecordDialog.TextInput(booking.Status)),
            ("Total (EGP)", RecordDialog.TextInput(booking.TotalAmount.ToString("N2"))),
            ("Paid (EGP)", RecordDialog.TextInput(booking.PaidAmount.ToString("N2"))),
            ("Remaining (EGP)", RecordDialog.TextInput(booking.RemainingAmount.ToString("N2"))),
            ("Payment status", RecordDialog.TextInput(booking.PaymentStatus)),
            ("Special requests", RecordDialog.TextInput(booking.SpecialRequests)));
        dialog.ShowDialog(this);
    }
}
