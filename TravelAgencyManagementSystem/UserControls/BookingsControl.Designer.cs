#nullable enable

namespace TravelAgencyManagementSystem.UserControls;

partial class BookingsControl
{
    private System.ComponentModel.IContainer? components;
    private RecordsPanel recordsPanel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        recordsPanel = new RecordsPanel();
        SuspendLayout();
        recordsPanel.Dock = DockStyle.Fill;
        recordsPanel.Margin = new Padding(0);
        recordsPanel.Name = "recordsPanel";
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Segoe UI", 10F);
        Size = new Size(1060, 668);
        Controls.Add(recordsPanel);
        Name = "BookingsControl";
        ResumeLayout(false);
    }
}
