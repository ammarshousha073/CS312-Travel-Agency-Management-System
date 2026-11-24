using TravelAgencyManagementSystem.Styling;

namespace TravelAgencyManagementSystem.UserControls;

public partial class RecordsPanel : UserControl
{
    private int groupColumn;
    private string tableTitle = "Records";
    public event EventHandler? AddRequested;
    public event EventHandler? EditRequested;
    public event EventHandler? DetailsRequested;
    public event EventHandler? DeleteRequested;

    public RecordsPanel()
    {
        InitializeComponent();
        AppTheme.StylePrimaryButton(addButton);
        AppTheme.StyleSecondaryButton(viewDetailsButton);
        AppTheme.StyleSecondaryButton(editButton);
        AppTheme.StyleSecondaryButton(deleteButton);
        deleteButton.ForeColor = Color.FromArgb(183, 65, 77);
        AppTheme.StyleGrid(recordsGrid);
        tablePanel.SizeChanged += (_, _) => tablePanel.Invalidate(true);
        groupComboBox.SizeChanged += (_, _) => groupComboBox.Invalidate();
        statusComboBox.SizeChanged += (_, _) => statusComboBox.Invalidate();
    }

    public string[]? SelectedRecord => recordsGrid.CurrentRow is { Visible: true } row
        ? row.Cells.Cast<DataGridViewCell>().Select(cell => cell.Value?.ToString() ?? "").ToArray()
        : null;

    public void Configure(string title, string description, string addText, string[] headers, string[][] rows)
    {
        headingLabel.Text = title;
        descriptionLabel.Text = description;
        addButton.Text = addText;
        addButton.Visible = addText.Length > 0;
        tableTitle = title;
        recordsGrid.Columns.Clear();
        for (var index = 0; index < headers.Length; index++)
            recordsGrid.Columns.Add($"Column{index}", headers[index]);
        foreach (var row in rows)
            recordsGrid.Rows.Add(row.Cast<object>().ToArray());
        recordsGrid.AccessibleName = title;
        groupComboBox.Items.Add("All categories");
        groupComboBox.SelectedIndex = 0;
        statusComboBox.Items.Add("All statuses");
        statusComboBox.SelectedIndex = 0;
        SetColumnWidths(Enumerable.Repeat(100F, headers.Length).ToArray());
    }

    public void ReplaceRows(string[][] rows)
    {
        previewLabel.Text = "Local MongoDB records";
        var group = groupComboBox.Text;
        recordsGrid.Rows.Clear();
        foreach (var row in rows)
            recordsGrid.Rows.Add(row.Cast<object>().ToArray());
        SetGroupFilter(groupLabel.Text, groupColumn);
        if (groupComboBox.Items.Contains(group))
            groupComboBox.SelectedItem = group;
        Filters_Changed(this, EventArgs.Empty);
    }

    internal void SetNotice(string text) => previewLabel.Text = text;

    public void SetWriteAccess(bool allowed)
    {
        addButton.Enabled = allowed;
        editButton.Visible &= allowed;
        deleteButton.Visible &= allowed;
    }

    public void SetColumnWidths(params float[] weights)
    {
        for (var index = 0; index < Math.Min(weights.Length, recordsGrid.Columns.Count); index++)
            recordsGrid.Columns[index].FillWeight = weights[index];
    }

    public void SetGroupFilter(string label, int column)
    {
        groupColumn = column;
        groupLabel.Text = label;
        groupComboBox.AccessibleName = $"{label} filter";
        groupComboBox.Items.Clear();
        groupComboBox.Items.Add(label == "Nationality" ? "All nationalities" : $"All {label.ToLowerInvariant()}s");
        groupComboBox.Items.AddRange(recordsGrid.Rows.Cast<DataGridViewRow>()
            .Select(row => row.Cells[column].Value?.ToString() ?? "").Distinct().Order().Cast<object>().ToArray());
        groupComboBox.SelectedIndex = 0;
    }

    public void SetStatusFilter(params string[] statuses)
    {
        statusComboBox.Items.Clear();
        statusComboBox.Items.Add("All statuses");
        statusComboBox.Items.AddRange(statuses);
        statusComboBox.SelectedIndex = 0;
    }

    public void SetActions(bool edit, bool delete)
    {
        editButton.Visible = edit;
        deleteButton.Visible = delete;
    }

    internal void SetDeleteText(string text)
    {
        deleteButton.Text = text;
        deleteButton.Width = Math.Max(deleteButton.Width, TextRenderer.MeasureText(text, deleteButton.Font).Width + 24);
    }

    public void SetSummary(params (string Label, string Value)[] metrics)
    {
        while (summaryLayout.Controls.Count > 0)
            summaryLayout.Controls[0].Dispose();
        summaryLayout.ColumnStyles.Clear();
        summaryLayout.RowStyles.Clear();
        pageLayout.RowStyles[1].Height = 106 * DeviceDpi / 96f;
        summaryLayout.Visible = true;
        summaryLayout.ColumnCount = metrics.Length;
        summaryLayout.RowCount = 1;
        summaryLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        foreach (var metric in metrics)
        {
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / metrics.Length));
            var card = new MetricCard(metric.Label, metric.Value, "")
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, summaryLayout.Controls.Count == metrics.Length - 1 ? 0 : 14, 0)
            };
            summaryLayout.Controls.Add(card);
        }
    }

    private void Filters_Changed(object? sender, EventArgs e)
    {
        if (recordsGrid.Columns.Count == 0)
            return;
        var search = searchTextBox.Text.Trim();
        recordsGrid.CurrentCell = null;
        foreach (DataGridViewRow row in recordsGrid.Rows)
        {
            row.Visible = row.Cells.Cast<DataGridViewCell>()
                .Any(cell => (cell.Value?.ToString() ?? "").Contains(search, StringComparison.OrdinalIgnoreCase))
                && (groupComboBox.SelectedIndex <= 0 || row.Cells[groupColumn].Value?.ToString() == groupComboBox.Text)
                && (statusComboBox.SelectedIndex <= 0 || row.Cells[row.Cells.Count - 1].Value?.ToString() == statusComboBox.Text);
        }
        var visibleRows = recordsGrid.Rows.Cast<DataGridViewRow>().Where(row => row.Visible).ToList();
        if (visibleRows.Count > 0)
            recordsGrid.CurrentCell = visibleRows[0].Cells[0];
        countLabel.Text = $"{visibleRows.Count} of {recordsGrid.Rows.Count} records";
        tableTitleLabel.Text = visibleRows.Count == 0 ? "No records match these filters" : tableTitle;
        UpdateActions();
    }

    private void UpdateActions()
    {
        var selected = SelectedRecord is not null;
        viewDetailsButton.Enabled = selected;
        editButton.Enabled = selected;
        deleteButton.Enabled = selected;
    }

    private void AddButton_Click(object? sender, EventArgs e) => AddRequested?.Invoke(this, e);
    private void EditButton_Click(object? sender, EventArgs e) => EditRequested?.Invoke(this, e);
    private void ViewDetailsButton_Click(object? sender, EventArgs e) => DetailsRequested?.Invoke(this, e);
    private void DeleteButton_Click(object? sender, EventArgs e) => DeleteRequested?.Invoke(this, e);
    private void RecordsGrid_SelectionChanged(object? sender, EventArgs e) => UpdateActions();

    private void RecordsGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
            DetailsRequested?.Invoke(this, EventArgs.Empty);
    }

    private void RecordsGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.ColumnIndex != recordsGrid.Columns.Count - 1 || e.CellStyle is null)
            return;
        e.CellStyle.ForeColor = e.Value?.ToString() switch
        {
            "Open" or "Active" or "Confirmed" or "Paid" => Color.FromArgb(21, 128, 103),
            "Full" or "Pending" or "Partial" => Color.FromArgb(166, 111, 20),
            "Cancelled" or "Inactive" or "Unpaid" or "Refunded" => Color.FromArgb(183, 65, 77),
            _ => AppTheme.Text
        };
        e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
    }

    private void TablePanel_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Control panel)
            return;
        using var border = new Pen(AppTheme.Border);
        e.Graphics.DrawRectangle(border, 0, 0, panel.Width - 1, panel.Height - 1);
    }
}

