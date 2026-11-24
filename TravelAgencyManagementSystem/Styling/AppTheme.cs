namespace TravelAgencyManagementSystem.Styling;

internal static class AppTheme
{
    public static readonly Color Navy = Color.FromArgb(23, 43, 77);
    public static readonly Color Teal = Color.FromArgb(32, 168, 154);
    public static readonly Color Background = Color.FromArgb(244, 247, 250);
    public static readonly Color Text = Color.FromArgb(38, 52, 69);
    public static readonly Color SecondaryText = Color.FromArgb(113, 128, 150);
    public static readonly Color Border = Color.FromArgb(226, 232, 240);

    public static void StylePrimaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(26, 146, 134);
        button.BackColor = Teal;
        button.ForeColor = Color.White;
        button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
    }

    public static void StyleSecondaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderColor = Border;
        button.FlatAppearance.MouseOverBackColor = Background;
        button.BackColor = Color.White;
        button.ForeColor = Text;
        button.Font = new Font("Segoe UI", 9.5F);
        button.Cursor = Cursors.Hand;
    }

    public static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.GridColor = Color.FromArgb(238, 242, 246);
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.ColumnHeadersHeight = 42;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        grid.RowTemplate.Height = 44;
        grid.RowHeadersVisible = false;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.AllowUserToResizeColumns = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Background,
            ForeColor = SecondaryText,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            SelectionBackColor = Background,
            SelectionForeColor = SecondaryText,
            Padding = new Padding(8, 0, 0, 0)
        };
        grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.White,
            ForeColor = Text,
            Font = new Font("Segoe UI", 9F),
            SelectionBackColor = Color.FromArgb(231, 246, 243),
            SelectionForeColor = Text,
            Padding = new Padding(8, 0, 0, 0)
        };
    }
}
