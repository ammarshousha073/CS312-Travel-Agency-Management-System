using System.Drawing.Drawing2D;

namespace TravelAgencyManagementSystem.UserControls;

public class TravelBrandPanel : Panel
{
    public TravelBrandPanel()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var graphics = e.Graphics;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var scale = DeviceDpi / 96f;
        var centerX = ClientSize.Width / 2f;

        var savedState = graphics.Save();
        graphics.TranslateTransform(centerX, 146 * scale);
        graphics.ScaleTransform(scale, scale);
        graphics.RotateTransform(-22);
        PointF[] airplane =
        [
            new(0, -31), new(5, -23), new(6, -7), new(31, 10),
            new(31, 16), new(6, 7), new(5, 24), new(13, 31),
            new(13, 35), new(0, 31), new(-13, 35), new(-13, 31),
            new(-5, 24), new(-6, 7), new(-31, 16), new(-31, 10),
            new(-6, -7), new(-5, -23)
        ];
        graphics.FillPolygon(Brushes.White, airplane);
        graphics.Restore(savedState);

        var routeY = ClientSize.Height - 141 * scale;
        var left = 54 * scale;
        var right = ClientSize.Width - 54 * scale;
        using var routePen = new Pen(Color.FromArgb(155, 184, 208), 1.5f * scale)
        {
            DashPattern = [3, 4]
        };
        graphics.DrawBezier(routePen,
            new PointF(left, routeY + 24 * scale),
            new PointF(centerX - 35 * scale, routeY - 110 * scale),
            new PointF(centerX + 25 * scale, routeY + 112 * scale),
            new PointF(right, routeY - 10 * scale));

        using var markerBrush = new SolidBrush(Color.FromArgb(32, 168, 154));
        graphics.FillEllipse(markerBrush, left - 5 * scale, routeY + 19 * scale, 10 * scale, 10 * scale);
        graphics.FillEllipse(Brushes.White, right - 7 * scale, routeY - 17 * scale, 14 * scale, 14 * scale);
        graphics.FillEllipse(markerBrush, right - 3 * scale, routeY - 13 * scale, 6 * scale, 6 * scale);
    }
}
