using TravelAgencyManagementSystem.Data;
using TravelAgencyManagementSystem.Forms;
using TravelAgencyManagementSystem.Styling;

namespace TravelAgencyManagementSystem.UserControls;

public partial class ReportsControl : UserControl
{
    private readonly RevenueChart revenueChart = new();
    private readonly BookingsChart bookingsChart = new();

    public ReportsControl()
    {
        InitializeComponent();
        AppTheme.StylePrimaryButton(generateButton);
        generateButton.Text = "Generate";
        toDatePicker.Value = DateTime.Today;
        fromDatePicker.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-5);
        previewLabel.Text = "";
        AddChartCard("Revenue by month", "Received payments in Egyptian pounds", revenueChart);
        AddChartCard("Booking activity", "Reservations by booking date", bookingsChart);
        AppTheme.StyleGrid(destinationsGrid);
        destinationsGrid.RowTemplate.Height = 32;
        destinationsGrid.ColumnHeadersHeight = 34;
        destinationsGrid.Columns.Add("Destination", "Destination");
        destinationsGrid.Columns.Add("Bookings", "Bookings");
        destinationsGrid.Columns.Add("Share", "Share of bookings");
        destinationsGrid.Columns[0].FillWeight = 170;
    }

    internal Task<bool> RefreshAsync() => UiOperation.RunAsync(this, async () =>
    {
        var report = await AgencyStore.Current.GenerateReportAsync(fromDatePicker.Value, toDatePicker.Value);
        if (IsDisposed)
            return;
        while (metricsLayout.Controls.Count > 0)
            metricsLayout.Controls[0].Dispose();
        AddMetric("Total revenue", $"EGP {report.Revenue:N2}", "Received during this period", "\uE8C7");
        AddMetric("Total bookings", report.Bookings.ToString("N0"), "Includes cancellations", "\uE8F1");
        AddMetric("Total clients", report.Clients.ToString("N0"), "Distinct clients with bookings", "\uE716");

        revenueChart.SetRevenue(report.Months.Select(month => (month.Month.ToString("MMM yy"), month.Revenue)).ToArray());
        bookingsChart.SetBookings(report.Months.Select(month => (month.Month.ToString("MMM yy"), month.Bookings)).ToArray());
        destinationsGrid.Rows.Clear();
        foreach (var destination in report.Destinations)
            destinationsGrid.Rows.Add(destination.Name, destination.Bookings.ToString("N0"),
                ((decimal)destination.Bookings / report.Bookings).ToString("P0"));
        destinationsGrid.ClearSelection();
        previewLabel.Text = report.Bookings == 0 && report.Revenue == 0
            ? "No bookings or payments in this date range."
            : $"Generated for {fromDatePicker.Value:dd MMM yyyy} – {toDatePicker.Value:dd MMM yyyy}";
    });

    private void AddMetric(string title, string value, string caption, string icon)
    {
        metricsLayout.Controls.Add(new MetricCard(title, value, caption, icon)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, metricsLayout.Controls.Count == 2 ? 0 : 14, 0)
        });
    }

    private void AddChartCard(string title, string caption, Control chart)
    {
        var card = new Panel
        {
            Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(18),
            Margin = new Padding(0, 0, chartsLayout.Controls.Count == 0 ? 16 : 0, 0)
        };
        chart.Dock = DockStyle.Fill;
        card.Controls.Add(chart);
        card.Controls.Add(new Label
        {
            Text = caption, Dock = DockStyle.Top, Height = 30,
            Font = new Font("Segoe UI", 9F), ForeColor = AppTheme.SecondaryText
        });
        card.Controls.Add(new Label
        {
            Text = title, Dock = DockStyle.Top, Height = 32,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = AppTheme.Navy
        });
        card.Paint += Card_Paint;
        chartsLayout.Controls.Add(card);
    }

    private async void GenerateButton_Click(object? sender, EventArgs e) => await RefreshAsync();

    private void Card_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Control card)
            return;
        using var border = new Pen(AppTheme.Border);
        e.Graphics.DrawRectangle(border, 0, 0, card.Width - 1, card.Height - 1);
    }
}
