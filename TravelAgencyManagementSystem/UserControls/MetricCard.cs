using TravelAgencyManagementSystem.Styling;

namespace TravelAgencyManagementSystem.UserControls;

public class MetricCard : UserControl
{
    public MetricCard() : this("Summary", "0", "")
    {
    }

    public MetricCard(string title, string value, string caption, string symbol = "")
    {
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.White;
        Padding = caption.Length == 0 ? new Padding(18, 10, 18, 10) : new Padding(18, 14, 18, 12);
        Size = new Size(250, 126);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Margin = new Padding(0) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, caption.Length == 0 ? 24F : 26F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, caption.Length == 0 ? 42F : 46F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        var titlePanel = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
        var titleLabel = new Label
        {
            Text = title, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9.5F), ForeColor = AppTheme.SecondaryText
        };
        var iconLabel = new Label
        {
            Text = symbol, Dock = DockStyle.Right, Width = symbol.Length > 0 ? 28 : 0,
            Font = new Font("Segoe MDL2 Assets", 15F), ForeColor = AppTheme.Teal, TextAlign = ContentAlignment.TopRight
        };
        titlePanel.Controls.Add(titleLabel);
        titlePanel.Controls.Add(iconLabel);
        layout.Controls.Add(titlePanel, 0, 0);
        layout.Controls.Add(new Label
        {
            Text = value, Dock = DockStyle.Fill, Margin = new Padding(0),
            Font = new Font("Segoe UI", value.Length > 6 ? 19F : 25F, FontStyle.Bold), ForeColor = AppTheme.Navy
        }, 0, 1);
        layout.Controls.Add(new Label
        {
            Text = caption, Dock = DockStyle.Fill, Margin = new Padding(0),
            Font = new Font("Segoe UI", 8.5F), ForeColor = AppTheme.SecondaryText
        }, 0, 2);
        Controls.Add(layout);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var border = new Pen(AppTheme.Border);
        e.Graphics.DrawRectangle(border, 0, 0, Width - 1, Height - 1);
    }
}
