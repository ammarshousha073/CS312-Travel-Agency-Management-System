using MongoDB.Bson;
using TravelAgencyManagementSystem.Data;
using TravelAgencyManagementSystem.Forms;

namespace TravelAgencyManagementSystem.UserControls;

public partial class PaymentsControl : UserControl
{
    private Account? account;
    private List<PaymentRow> rows = [];
    internal event EventHandler? ReservationsChanged;

    public PaymentsControl()
    {
        InitializeComponent();
        recordsPanel.Configure("Payments overview", "Review EGP payments, outstanding balances, and receipts.", "+  Register payment",
            ["Payment ID", "Booking ID", "Client", "Amount", "Method", "Payment date", "Receipt", "Status"], []);
        recordsPanel.SetColumnWidths(135, 135, 105, 85, 90, 85, 135, 65);
        recordsPanel.SetGroupFilter("Method", 4);
        recordsPanel.SetStatusFilter("Paid", "Partial", "Unpaid");
        recordsPanel.SetActions(false, false);
        recordsPanel.SetSummary(("Total paid", "—"), ("Remaining balance", "—"));
        recordsPanel.AddRequested += async (_, _) => await RegisterAsync();
        recordsPanel.DetailsRequested += (_, _) => ShowDetails();
    }

    internal Task<bool> RefreshAsync(Account actor) => UiOperation.RunAsync(this, async () =>
    {
        account = actor;
        recordsPanel.SetWriteAccess(actor.Role is "Administrator" or "Accountant");
        var snapshot = await AgencyStore.Current.LoadReservationsAsync();
        if (IsDisposed)
            return;
        rows = snapshot.Payments;
        recordsPanel.ReplaceRows(rows.Select(row => new[]
        {
            row.Payment.PaymentNumber, row.BookingNumber, row.Client, row.Payment.Amount.ToString("N2"),
            row.Payment.PaymentMethod, row.Payment.PaymentDate.ToLocalTime().ToString("dd MMM yyyy"),
            row.Payment.ReceiptReference, row.Payment.Status
        }).ToArray());
        recordsPanel.SetSummary(("Total paid", $"EGP {snapshot.Revenue:N2}"),
            ("Remaining balance", $"EGP {snapshot.Outstanding:N2}"));
    });

    private async Task RegisterAsync()
    {
        if (account is not { } actor)
            return;
        List<BookingRow> available = [];
        if (!await UiOperation.RunAsync(this, async () =>
        {
            var snapshot = await AgencyStore.Current.LoadReservationsAsync();
            available = snapshot.Bookings.Where(row => row.Booking.Status != "Cancelled" && row.Booking.RemainingAmount > 0).ToList();
        }) || IsDisposed)
            return;
        if (available.Count == 0)
        {
            MessageBox.Show(this, "There are no active bookings with an outstanding balance.", "Register payment");
            return;
        }

        var booking = RecordDialog.ChoiceInput("", available.Select(row => $"{row.Client} · {row.Booking.BookingNumber}").ToArray());
        var total = RecordDialog.TextInput();
        var paid = RecordDialog.TextInput();
        var due = RecordDialog.TextInput();
        total.ReadOnly = paid.ReadOnly = due.ReadOnly = true;
        var amount = new NumericUpDown { DecimalPlaces = 2, Minimum = 0, ThousandsSeparator = true };
        var method = RecordDialog.ChoiceInput("", AgencyStore.PaymentMethods.ToArray());
        void UpdateBalance()
        {
            var selected = available[booking.SelectedIndex].Booking;
            total.Text = selected.TotalAmount.ToString("N2");
            paid.Text = selected.PaidAmount.ToString("N2");
            due.Text = selected.RemainingAmount.ToString("N2");
            amount.Maximum = selected.RemainingAmount;
            amount.Value = 0;
        }
        booking.SelectedIndexChanged += (_, _) => UpdateBalance();
        UpdateBalance();

        var requestId = ObjectId.GenerateNewId();
        using var dialog = new RecordDialog("Register payment", "Record a payment received by the agency", false,
            ("Booking", booking), ("Total (EGP)", total), ("Already paid (EGP)", paid), ("Remaining (EGP)", due),
            ("Payment amount (EGP)", amount), ("Payment method", method));
        dialog.SaveAsync = () => AgencyStore.Current.RegisterPaymentAsync(actor.Id, requestId,
            available[booking.SelectedIndex].Booking.Id, amount.Value, method.Text);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            await RefreshAsync(actor);
            if (!IsDisposed)
                ReservationsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void ShowDetails()
    {
        var row = rows.Find(item => item.Payment.PaymentNumber == recordsPanel.SelectedRecord?[0]);
        if (row is null)
            return;
        var payment = row.Payment;
        using var dialog = new RecordDialog("Payment details", payment.PaymentNumber, true,
            ("Booking number", RecordDialog.TextInput(row.BookingNumber)), ("Client", RecordDialog.TextInput(row.Client)),
            ("Amount (EGP)", RecordDialog.TextInput(payment.Amount.ToString("N2"))),
            ("Payment date", RecordDialog.TextInput(payment.PaymentDate.ToLocalTime().ToString("dd MMM yyyy HH:mm"))),
            ("Payment method", RecordDialog.TextInput(payment.PaymentMethod)),
            ("Payment status", RecordDialog.TextInput(payment.Status)),
            ("Receipt reference", RecordDialog.TextInput(payment.ReceiptReference)));
        dialog.ShowDialog(this);
    }
}
