using TravelAgencyManagementSystem.Data;
using TravelAgencyManagementSystem.Styling;
using TravelAgencyManagementSystem.UserControls;

namespace TravelAgencyManagementSystem.Forms;

public partial class MainForm : Form
{
    private readonly Dictionary<string, Control> pages = new();
    internal Account? Account { get; init; }

    public MainForm()
    {
        InitializeComponent();
        pages.Add("Dashboard", dashboardControl);
        AddNavigation();
        SizeChanged += (_, _) => contentPanel.Invalidate(true);
        ResizeEnd += (_, _) => contentPanel.Refresh();
    }

    private void AddNavigation()
    {
        string[] sections = ["Dashboard", "Trips", "Clients", "Bookings", "Payments", "Reports", "Users"];
        string[] symbols = ["\uE80F", "\uE709", "\uE716", "\uE8F1", "\uE8C7", "\uE9D9", "\uE77B"];

        for (var index = 0; index < sections.Length; index++)
        {
            var item = new Button
            {
                Name = $"{sections[index].ToLowerInvariant()}NavigationButton",
                Text = sections[index],
                Tag = sections[index],
                Width = 188,
                Height = 46,
                Margin = new Padding(0, 0, 0, 7),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(44, 0, 0, 0),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = index == 0 ? AppTheme.Teal : AppTheme.Navy,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10F)
            };
            item.FlatAppearance.BorderSize = 0;
            item.FlatAppearance.MouseOverBackColor = index == 0 ? AppTheme.Teal : Color.FromArgb(36, 57, 91);
            var symbol = symbols[index];
            item.Paint += (_, e) => DrawNavigationIcon(e.Graphics, symbol, item.ClientSize.Height);
            item.Click += NavigationButton_Click;
            navigationPanel.Controls.Add(item);
        }

        navigationPanel.SizeChanged += (_, _) => ResizeNavigationButtons();
        Shown += async (_, _) =>
        {
            ResizeNavigationButtons();
            if (Account is { } account)
            {
                profileLabel.Text = $"{account.Username}   ·   {account.Role}";
                await dashboardControl.RefreshAsync();
            }
        };
    }

    private void ResizeNavigationButtons()
    {
        var scale = DeviceDpi / 96f;
        foreach (Control item in navigationPanel.Controls)
        {
            item.Width = navigationPanel.ClientSize.Width;
            item.Height = (int)(46 * scale);
            item.Padding = new Padding((int)(44 * scale), 0, 0, 0);
            item.Margin = new Padding(0, 0, 0, (int)(7 * scale));
        }
    }

    private void DrawNavigationIcon(Graphics graphics, string symbol, int height)
    {
        using var iconFont = new Font("Segoe MDL2 Assets", 13F);
        var scale = DeviceDpi / 96f;
        TextRenderer.DrawText(graphics, symbol, iconFont,
            new Rectangle((int)(12 * scale), 0, (int)(24 * scale), height),
            Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
    }

    private async void NavigationButton_Click(object? sender, EventArgs e)
    {
        if (sender is not Button { Tag: string section })
            return;
        if (!pages.TryGetValue(section, out var page))
        {
            page = section switch
            {
                "Trips" => new TripsControl(),
                "Clients" => new ClientsControl(),
                "Bookings" => new BookingsControl(),
                "Payments" => new PaymentsControl(),
                "Reports" => new ReportsControl(),
                "Users" => new UsersControl(),
                _ => dashboardControl
            };
            if (page is BookingsControl bookings)
                bookings.ReservationsChanged += ReservationsChanged;
            if (page is PaymentsControl payments)
                payments.ReservationsChanged += ReservationsChanged;
            page.Dock = DockStyle.Fill;
            page.Margin = new Padding(0);
            contentPanel.Controls.Add(page);
            pages.Add(section, page);
        }
        contentPanel.SuspendLayout();
        foreach (Control content in contentPanel.Controls)
            content.Visible = content == page;
        page.BringToFront();
        contentPanel.ResumeLayout();
        pageTitleLabel.Text = section;
        foreach (Button item in navigationPanel.Controls)
        {
            var selected = item.Tag?.ToString() == section;
            item.BackColor = selected ? AppTheme.Teal : AppTheme.Navy;
            item.FlatAppearance.MouseOverBackColor = selected ? AppTheme.Teal : Color.FromArgb(36, 57, 91);
        }
        if (Account is { } account)
        {
            if (page is DashboardControl dashboard)
                await dashboard.RefreshAsync();
            if (page is TripsControl trips)
                await trips.RefreshAsync(account);
            if (page is ClientsControl clients)
                await clients.RefreshAsync(account);
            if (page is BookingsControl bookings)
                await bookings.RefreshAsync(account);
            if (page is PaymentsControl payments)
                await payments.RefreshAsync(account);
            if (page is ReportsControl reports)
                await reports.RefreshAsync();
            if (page is UsersControl users)
                await users.RefreshAsync(account);
        }
    }
    private async void ReservationsChanged(object? sender, EventArgs e)
    {
        if (IsDisposed || Account is not { } account)
            return;
        var refreshes = new List<Task> { dashboardControl.RefreshAsync() };
        if (sender is not BookingsControl && pages.GetValueOrDefault("Bookings") is BookingsControl bookings)
            refreshes.Add(bookings.RefreshAsync(account));
        if (sender is not PaymentsControl && pages.GetValueOrDefault("Payments") is PaymentsControl payments)
            refreshes.Add(payments.RefreshAsync(account));
        await Task.WhenAll(refreshes);
    }

    private void SignOutButton_Click(object? sender, EventArgs e)
    {
        Close();
    }
}

