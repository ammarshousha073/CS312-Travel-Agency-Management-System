#nullable enable

namespace TravelAgencyManagementSystem.UserControls;

partial class DashboardControl
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel pageLayout = null!;
    private Panel welcomePanel = null!;
    private Label welcomeLabel = null!;
    private Label descriptionLabel = null!;
    private TableLayoutPanel metricsLayout = null!;
    private TableLayoutPanel detailsLayout = null!;
    private Panel bookingsPanel = null!;
    private Label bookingsTitleLabel = null!;
    private Label bookingsCaptionLabel = null!;
    private DataGridView bookingsGrid = null!;
    private Panel revenuePanel = null!;
    private Label revenueTitleLabel = null!;
    private Label revenueCaptionLabel = null!;
    private RevenueChart revenueChart = null!;
    private Label sampleDataLabel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pageLayout = new TableLayoutPanel();
        welcomePanel = new Panel();
        welcomeLabel = new Label();
        descriptionLabel = new Label();
        metricsLayout = new TableLayoutPanel();
        detailsLayout = new TableLayoutPanel();
        bookingsPanel = new Panel();
        bookingsTitleLabel = new Label();
        bookingsCaptionLabel = new Label();
        bookingsGrid = new DataGridView();
        revenuePanel = new Panel();
        revenueTitleLabel = new Label();
        revenueCaptionLabel = new Label();
        revenueChart = new RevenueChart();
        sampleDataLabel = new Label();
        pageLayout.SuspendLayout();
        welcomePanel.SuspendLayout();
        detailsLayout.SuspendLayout();
        bookingsPanel.SuspendLayout();
        revenuePanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)bookingsGrid).BeginInit();
        SuspendLayout();

        pageLayout.ColumnCount = 1;
        pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        pageLayout.RowCount = 4;
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 95F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 142F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        pageLayout.Dock = DockStyle.Fill;
        pageLayout.Padding = new Padding(28, 14, 28, 10);
        pageLayout.Controls.Add(welcomePanel, 0, 0);
        pageLayout.Controls.Add(metricsLayout, 0, 1);
        pageLayout.Controls.Add(detailsLayout, 0, 2);
        pageLayout.Controls.Add(sampleDataLabel, 0, 3);
        pageLayout.Name = "pageLayout";

        welcomePanel.Dock = DockStyle.Fill;
        welcomePanel.Margin = new Padding(0);
        welcomePanel.Controls.Add(welcomeLabel);
        welcomePanel.Controls.Add(descriptionLabel);
        welcomePanel.Name = "welcomePanel";
        welcomeLabel.AutoSize = true;
        welcomeLabel.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
        welcomeLabel.ForeColor = Color.FromArgb(23, 43, 77);
        welcomeLabel.Location = new Point(0, 3);
        welcomeLabel.Text = "Welcome back";
        welcomeLabel.Name = "welcomeLabel";
        descriptionLabel.AutoSize = true;
        descriptionLabel.ForeColor = Color.FromArgb(113, 128, 150);
        descriptionLabel.Location = new Point(2, 53);
        descriptionLabel.Text = "A clear view of your agency, all in one place.";
        descriptionLabel.Name = "descriptionLabel";

        metricsLayout.ColumnCount = 4;
        metricsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        metricsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        metricsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        metricsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        metricsLayout.RowCount = 1;
        metricsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        metricsLayout.Dock = DockStyle.Fill;
        metricsLayout.Margin = new Padding(0, 0, 0, 16);
        metricsLayout.Name = "metricsLayout";

        detailsLayout.ColumnCount = 2;
        detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64F));
        detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36F));
        detailsLayout.RowCount = 1;
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        detailsLayout.Dock = DockStyle.Fill;
        detailsLayout.Margin = new Padding(0);
        detailsLayout.Controls.Add(bookingsPanel, 0, 0);
        detailsLayout.Controls.Add(revenuePanel, 1, 0);
        detailsLayout.Name = "detailsLayout";

        bookingsPanel.BackColor = Color.White;
        bookingsPanel.Dock = DockStyle.Fill;
        bookingsPanel.Padding = new Padding(18);
        bookingsPanel.Margin = new Padding(0, 0, 16, 0);
        bookingsPanel.Controls.Add(bookingsGrid);
        bookingsPanel.Controls.Add(bookingsCaptionLabel);
        bookingsPanel.Controls.Add(bookingsTitleLabel);
        bookingsPanel.Paint += Card_Paint;
        bookingsPanel.Name = "bookingsPanel";
        bookingsTitleLabel.Dock = DockStyle.Top;
        bookingsTitleLabel.Height = 31;
        bookingsTitleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        bookingsTitleLabel.Text = "Recent bookings";
        bookingsTitleLabel.Name = "bookingsTitleLabel";
        bookingsCaptionLabel.Dock = DockStyle.Top;
        bookingsCaptionLabel.Height = 35;
        bookingsCaptionLabel.ForeColor = Color.FromArgb(113, 128, 150);
        bookingsCaptionLabel.Font = new Font("Segoe UI", 9F);
        bookingsCaptionLabel.Text = "The latest reservations at a glance";
        bookingsCaptionLabel.Name = "bookingsCaptionLabel";

        bookingsGrid.Dock = DockStyle.Fill;
        bookingsGrid.BackgroundColor = Color.White;
        bookingsGrid.BorderStyle = BorderStyle.None;
        bookingsGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        bookingsGrid.GridColor = Color.FromArgb(238, 242, 246);
        bookingsGrid.EnableHeadersVisualStyles = false;
        bookingsGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        bookingsGrid.ColumnHeadersHeight = 38;
        bookingsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        bookingsGrid.RowTemplate.Height = 39;
        bookingsGrid.RowHeadersVisible = false;
        bookingsGrid.ReadOnly = true;
        bookingsGrid.AllowUserToAddRows = false;
        bookingsGrid.AllowUserToDeleteRows = false;
        bookingsGrid.AllowUserToResizeRows = false;
        bookingsGrid.AllowUserToResizeColumns = false;
        bookingsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        bookingsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        bookingsGrid.MultiSelect = false;
        bookingsGrid.CellFormatting += BookingsGrid_CellFormatting;
        bookingsGrid.Name = "bookingsGrid";

        revenuePanel.BackColor = Color.White;
        revenuePanel.Dock = DockStyle.Fill;
        revenuePanel.Padding = new Padding(18);
        revenuePanel.Margin = new Padding(0);
        revenuePanel.Controls.Add(revenueChart);
        revenuePanel.Controls.Add(revenueCaptionLabel);
        revenuePanel.Controls.Add(revenueTitleLabel);
        revenuePanel.Paint += Card_Paint;
        revenuePanel.Name = "revenuePanel";
        revenueTitleLabel.Dock = DockStyle.Top;
        revenueTitleLabel.Height = 31;
        revenueTitleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        revenueTitleLabel.Text = "Revenue overview";
        revenueTitleLabel.Name = "revenueTitleLabel";
        revenueCaptionLabel.Dock = DockStyle.Top;
        revenueCaptionLabel.Height = 35;
        revenueCaptionLabel.ForeColor = Color.FromArgb(113, 128, 150);
        revenueCaptionLabel.Font = new Font("Segoe UI", 9F);
        revenueCaptionLabel.Text = "Last 6 months · EGP";
        revenueCaptionLabel.Name = "revenueCaptionLabel";
        revenueChart.Dock = DockStyle.Fill;
        revenueChart.BackColor = Color.White;
        revenueChart.Name = "revenueChart";

        sampleDataLabel.Dock = DockStyle.Fill;
        sampleDataLabel.TextAlign = ContentAlignment.BottomLeft;
        sampleDataLabel.Font = new Font("Segoe UI", 9F);
        sampleDataLabel.ForeColor = Color.FromArgb(113, 128, 150);
        sampleDataLabel.Text = "Sample data for the GUI presentation · June – November 2026";
        sampleDataLabel.Margin = new Padding(0);
        sampleDataLabel.Name = "sampleDataLabel";
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(244, 247, 250);
        ForeColor = Color.FromArgb(38, 52, 69);
        Font = new Font("Segoe UI", 10F);
        Size = new Size(1060, 668);
        Controls.Add(pageLayout);
        Name = "DashboardControl";
        pageLayout.ResumeLayout(false);
        welcomePanel.ResumeLayout(false);
        welcomePanel.PerformLayout();
        detailsLayout.ResumeLayout(false);
        bookingsPanel.ResumeLayout(false);
        revenuePanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)bookingsGrid).EndInit();
        ResumeLayout(false);
    }
}
