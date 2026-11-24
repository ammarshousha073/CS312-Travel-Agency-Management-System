using TravelAgencyManagementSystem.Styling;

namespace TravelAgencyManagementSystem.UserControls;

public class RevenueChart : Control
{
    private decimal[] monthlyRevenue = [];
    private string[] months = [];

    public RevenueChart()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        AccessibleName = "Revenue by month";
    }

    internal void SetRevenue((string Month, decimal Revenue)[] points)
    {
        months = points.Select(point => point.Month).ToArray();
        monthlyRevenue = points.Select(point => point.Revenue).ToArray();
        AccessibleName = "Recorded revenue by month";
        AccessibleDescription = string.Join("; ", points.Select(point => $"{point.Month} {point.Revenue:N2} EGP"));
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var scale = DeviceDpi / 96f;
        var plot = new RectangleF(38 * scale, 24 * scale, Width - 46 * scale, Height - 58 * scale);
        if (months.Length == 0 || plot.Width <= 0 || plot.Height <= 0)
            return;

        using var labelFont = new Font("Segoe UI", 8F);
        using var gridPen = new Pen(AppTheme.Border);
        using var barBrush = new SolidBrush(AppTheme.Teal);
        using var lastBarBrush = new SolidBrush(AppTheme.Navy);
        var step = Math.Max(1, decimal.Ceiling(monthlyRevenue.Max() / 4));
        var ceiling = step * 4;
        for (var index = 0; index <= 4; index++)
        {
            var amount = step * index;
            var y = plot.Bottom - index / 4f * plot.Height;
            e.Graphics.DrawLine(gridPen, plot.Left, y, plot.Right, y);
            var label = amount >= 1000 ? $"{amount / 1000:0.#}k" : $"{amount:0}";
            TextRenderer.DrawText(e.Graphics, label, labelFont,
                new Rectangle(0, (int)y - (int)(8 * scale), (int)(31 * scale), (int)(20 * scale)),
                AppTheme.SecondaryText, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        }

        var slotWidth = plot.Width / months.Length;
        var barWidth = slotWidth * 0.55f;
        var labelInterval = Math.Max(1, (int)Math.Ceiling(51 * scale / slotWidth));
        for (var index = 0; index < months.Length; index++)
        {
            var height = (float)(monthlyRevenue[index] / ceiling) * plot.Height;
            var x = plot.Left + index * slotWidth + (slotWidth - barWidth) / 2;
            e.Graphics.FillRectangle(index == months.Length - 1 ? lastBarBrush : barBrush,
                x, plot.Bottom - height, barWidth, height);
            if (index % labelInterval != 0)
                continue;
            TextRenderer.DrawText(e.Graphics, months[index], labelFont,
                new Rectangle((int)(plot.Left + (index + 0.5f) * slotWidth - 25 * scale), (int)(plot.Bottom + 9 * scale),
                    (int)(50 * scale), (int)(21 * scale)),
                AppTheme.SecondaryText, TextFormatFlags.HorizontalCenter);
        }
    }
}
