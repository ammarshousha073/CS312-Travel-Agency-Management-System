using TravelAgencyManagementSystem.Data;
using TravelAgencyManagementSystem.Forms;
using TravelAgencyManagementSystem.Styling;

namespace TravelAgencyManagementSystem.UserControls;

public partial class DashboardControl : UserControl
{
    public DashboardControl()
    {
        InitializeComponent();
        ShowMetrics("—", "—", "—", "—");
        revenueChart.Visible = false;
        sampleDataLabel.Text = "Loading local MongoDB records…";
        ConfigureBookingsTable();
    }

    internal Task<bool> RefreshAsync() => UiOperation.RunAsync(this, async () =>
    {
        var countsTask = AgencyStore.Current.CountRecordsAsync();
        var reservationsTask = AgencyStore.Current.LoadReservationsAsync();
        await Task.WhenAll(countsTask, reservationsTask);
        if (IsDisposed)
            return;

        var counts = await countsTask;
        var snapshot = await reservationsTask;
        ShowMetrics(counts.Trips.ToString("N0"), counts.Clients.ToString("N0"),
            snapshot.Bookings.Count.ToString("N0"), $"EGP {snapshot.Revenue:N2}");
        bookingsGrid.Rows.Clear();
        foreach (var row in snapshot.Bookings.Take(6))
            bookingsGrid.Rows.Add(row.Booking.BookingNumber, row.Client, row.Destination,
                row.Departure?.ToString("dd MMM yyyy") ?? "Unknown", row.Booking.Status);
        bookingsGrid.ClearSelection();
        revenueChart.SetRevenue(snapshot.MonthlyRevenue(DateTime.Today));
        revenueChart.Visible = true;
        sampleDataLabel.Text = snapshot.Bookings.Count == 0 ? "No bookings recorded yet." : "Local MongoDB records";
    });

    private void ShowMetrics(string trips, string clients, string bookings, string revenue)
    {
        while (metricsLayout.Controls.Count > 0)
            metricsLayout.Controls[0].Dispose();
        AddMetricCard("Total trips", trips, "All stored trips", "\uE709");
        AddMetricCard("Total clients", clients, "All stored clients", "\uE716");
        AddMetricCard("Total bookings", bookings, "Includes cancellations", "\uE8F1");
        AddMetricCard("Total revenue", revenue, "Recorded payments", "\uE8C7");
    }

    private void AddMetricCard(string title, string value, string caption, string symbol)
    {
        metricsLayout.Controls.Add(new MetricCard(title, value, caption, symbol)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, metricsLayout.Controls.Count == 3 ? 0 : 14, 0)
        });
    }
    private void ConfigureBookingsTable()
    {
        AppTheme.StyleGrid(bookingsGrid);
        bookingsGrid.RowTemplate.Height = 39;
        bookingsGrid.ColumnHeadersHeight = 38;
        bookingsGrid.Columns.Add("BookingId", "Booking ID");
        bookingsGrid.Columns.Add("Client", "Client");
        bookingsGrid.Columns.Add("Destination", "Destination");
        bookingsGrid.Columns.Add("Departure", "Departure");
        bookingsGrid.Columns.Add("Status", "Status");
        bookingsGrid.Columns[0].FillWeight = 85;
        bookingsGrid.Columns[1].FillWeight = 125;
        bookingsGrid.Columns[2].FillWeight = 105;
        bookingsGrid.Columns[3].FillWeight = 100;
        bookingsGrid.Columns[4].FillWeight = 95;
        bookingsGrid.ClearSelection();
    }

    private void BookingsGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.ColumnIndex != 4 || e.CellStyle is null)
            return;

        e.CellStyle.ForeColor = e.Value?.ToString() switch
        {
            "Confirmed" => Color.FromArgb(21, 128, 103),
            "Pending" => Color.FromArgb(166, 111, 20),
            "Cancelled" => Color.FromArgb(183, 65, 77),
            _ => AppTheme.Text
        };
        e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
    }

    private void Card_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Control card)
            return;
        using var border = new Pen(AppTheme.Border);
        e.Graphics.DrawRectangle(border, 0, 0, card.Width - 1, card.Height - 1);
    }
}

