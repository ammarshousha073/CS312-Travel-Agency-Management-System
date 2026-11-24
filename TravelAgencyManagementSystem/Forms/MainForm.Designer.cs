#nullable enable

using TravelAgencyManagementSystem.UserControls;

namespace TravelAgencyManagementSystem.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel shellLayout = null!;
    private Panel sidebarPanel = null!;
    private Label brandLabel = null!;
    private Label brandCaptionLabel = null!;
    private FlowLayoutPanel navigationPanel = null!;
    private Button signOutButton = null!;
    private Panel headerPanel = null!;
    private Label pageTitleLabel = null!;
    private Label profileLabel = null!;
    private DashboardControl dashboardControl = null!;
    private Panel contentPanel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        shellLayout = new TableLayoutPanel();
        sidebarPanel = new Panel();
        brandLabel = new Label();
        brandCaptionLabel = new Label();
        navigationPanel = new FlowLayoutPanel();
        signOutButton = new Button();
        headerPanel = new Panel();
        pageTitleLabel = new Label();
        profileLabel = new Label();
        dashboardControl = new DashboardControl();
        contentPanel = new Panel();
        shellLayout.SuspendLayout();
        sidebarPanel.SuspendLayout();
        headerPanel.SuspendLayout();
        SuspendLayout();

        shellLayout.Size = new Size(1280, 740);
        shellLayout.ColumnCount = 2;
        shellLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
        shellLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        shellLayout.RowCount = 2;
        shellLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
        shellLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        shellLayout.Dock = DockStyle.Fill;
        shellLayout.Margin = new Padding(0);
        shellLayout.Controls.Add(sidebarPanel, 0, 0);
        shellLayout.SetRowSpan(sidebarPanel, 2);
        shellLayout.Controls.Add(headerPanel, 1, 0);
        shellLayout.Controls.Add(contentPanel, 1, 1);
        shellLayout.Name = "shellLayout";

        sidebarPanel.Size = new Size(220, 740);
        sidebarPanel.Dock = DockStyle.Fill;
        sidebarPanel.Margin = new Padding(0);
        sidebarPanel.BackColor = Color.FromArgb(23, 43, 77);
        sidebarPanel.Controls.Add(brandLabel);
        sidebarPanel.Controls.Add(brandCaptionLabel);
        sidebarPanel.Controls.Add(navigationPanel);
        sidebarPanel.Controls.Add(signOutButton);
        sidebarPanel.Name = "sidebarPanel";

        brandLabel.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
        brandLabel.ForeColor = Color.White;
        brandLabel.Location = new Point(25, 28);
        brandLabel.Size = new Size(184, 40);
        brandLabel.Text = "Travel Agency";
        brandLabel.Name = "brandLabel";
        brandCaptionLabel.Font = new Font("Segoe UI", 9F);
        brandCaptionLabel.ForeColor = Color.FromArgb(167, 189, 213);
        brandCaptionLabel.Location = new Point(27, 72);
        brandCaptionLabel.Size = new Size(180, 28);
        brandCaptionLabel.Text = "MANAGEMENT SYSTEM";
        brandCaptionLabel.Name = "brandCaptionLabel";

        navigationPanel.Location = new Point(16, 142);
        navigationPanel.Size = new Size(188, 390);
        navigationPanel.FlowDirection = FlowDirection.TopDown;
        navigationPanel.WrapContents = false;
        navigationPanel.Name = "navigationPanel";

        signOutButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        signOutButton.Location = new Point(16, 668);
        signOutButton.Size = new Size(188, 44);
        signOutButton.BackColor = Color.FromArgb(23, 43, 77);
        signOutButton.ForeColor = Color.FromArgb(203, 216, 232);
        signOutButton.FlatStyle = FlatStyle.Flat;
        signOutButton.FlatAppearance.BorderColor = Color.FromArgb(58, 77, 107);
        signOutButton.Text = "Sign out";
        signOutButton.Cursor = Cursors.Hand;
        signOutButton.Click += SignOutButton_Click;
        signOutButton.Name = "signOutButton";

        headerPanel.Size = new Size(1060, 72);
        headerPanel.Dock = DockStyle.Fill;
        headerPanel.Margin = new Padding(0);
        headerPanel.BackColor = Color.White;
        headerPanel.Controls.Add(pageTitleLabel);
        headerPanel.Controls.Add(profileLabel);
        headerPanel.Name = "headerPanel";
        pageTitleLabel.AutoSize = true;
        pageTitleLabel.Location = new Point(28, 22);
        pageTitleLabel.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
        pageTitleLabel.Text = "Dashboard";
        pageTitleLabel.Name = "pageTitleLabel";
        profileLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        profileLabel.Location = new Point(822, 22);
        profileLabel.Size = new Size(210, 28);
        profileLabel.ForeColor = Color.FromArgb(113, 128, 150);
        profileLabel.Text = "Admin   ·   GUI preview";
        profileLabel.TextAlign = ContentAlignment.MiddleRight;
        profileLabel.Name = "profileLabel";

        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Margin = new Padding(0);
        contentPanel.Name = "contentPanel";
        contentPanel.Controls.Add(dashboardControl);
        dashboardControl.Dock = DockStyle.Fill;
        dashboardControl.Margin = new Padding(0);
        dashboardControl.Name = "dashboardControl";
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1280, 740);
        MinimumSize = new Size(1120, 720);
        Font = new Font("Segoe UI", 10F);
        ForeColor = Color.FromArgb(38, 52, 69);
        BackColor = Color.FromArgb(244, 247, 250);
        Controls.Add(shellLayout);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Travel Agency Management System";
        Name = "MainForm";
        shellLayout.ResumeLayout(false);
        sidebarPanel.ResumeLayout(false);
        headerPanel.ResumeLayout(false);
        headerPanel.PerformLayout();
        ResumeLayout(false);
    }
}
