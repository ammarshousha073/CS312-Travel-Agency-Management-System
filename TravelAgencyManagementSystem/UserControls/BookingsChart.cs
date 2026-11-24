using System.Drawing.Drawing2D;
using TravelAgencyManagementSystem.Styling;

namespace TravelAgencyManagementSystem.UserControls;

public class BookingsChart : Control
{
    private int[] counts = [];
    private string[] months = [];

    public BookingsChart()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        AccessibleName = "Bookings by month";
    }

    internal void SetBookings((string Month, int Bookings)[] points)
    {
        months = points.Select(point => point.Month).ToArray();
        counts = points.Select(point => point.Bookings).ToArray();
        AccessibleDescription = string.Join("; ", points.Select(point => $"{point.Month} {point.Bookings}"));
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var scale = DeviceDpi / 96f;
        var plot = new RectangleF(30 * scale, 20 * scale, Width - 50 * scale, Height - 60 * scale);
        if (counts.Length == 0 || plot.Width <= 0 || plot.Height <= 0)
            return;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var labelFont = new Font("Segoe UI", 8F);
        using var gridPen = new Pen(AppTheme.Border);
        using var linePen = new Pen(AppTheme.Teal, 2.5f * scale);
        using var dotBrush = new SolidBrush(AppTheme.Teal);
        var step = Math.Max(1, (int)Math.Ceiling(counts.Max() / 5d));
        for (var tick = 0; tick <= 5; tick++)
        {
            var count = (long)tick * step;
            var y = plot.Bottom - tick / 5f * plot.Height;
            e.Graphics.DrawLine(gridPen, plot.Left, y, plot.Right, y);
            TextRenderer.DrawText(e.Graphics, count.ToString(), labelFont,
                new Rectangle(0, (int)y - 8, (int)(25 * scale), (int)(20 * scale)),
                AppTheme.SecondaryText, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        }
        var slot = plot.Width / counts.Length;
        var labelInterval = Math.Max(1, (int)Math.Ceiling(51 * scale / slot));
        var points = counts.Select((count, index) => new PointF(
            plot.Left + (index + 0.5f) * slot, plot.Bottom - count / (5f * step) * plot.Height)).ToArray();
        if (points.Length > 1)
            e.Graphics.DrawLines(linePen, points);
        for (var index = 0; index < points.Length; index++)
        {
            var point = points[index];
            e.Graphics.FillEllipse(dotBrush, point.X - 4 * scale, point.Y - 4 * scale, 8 * scale, 8 * scale);
            if (index % labelInterval != 0)
                continue;
            TextRenderer.DrawText(e.Graphics, months[index], labelFont,
                new Rectangle((int)(point.X - 25 * scale), (int)(plot.Bottom + 12 * scale), (int)(50 * scale), (int)(22 * scale)),
                AppTheme.SecondaryText, TextFormatFlags.HorizontalCenter);
        }
    }
}
