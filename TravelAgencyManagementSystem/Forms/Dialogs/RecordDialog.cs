using TravelAgencyManagementSystem.Styling;

namespace TravelAgencyManagementSystem.Forms;

public partial class RecordDialog : Form
{
    public RecordDialog()
    {
        InitializeComponent();
        previewLabel.Visible = false;
        saveButton.Text = "Save";
        AppTheme.StylePrimaryButton(saveButton);
        AppTheme.StyleSecondaryButton(closeButton);
    }

    public RecordDialog(string title, string subtitle, bool readOnly, params (string Label, Control Input)[] fields) : this()
    {
        Text = title;
        titleLabel.Text = title;
        subtitleLabel.Text = subtitle;
        fieldsLayout.RowCount = (fields.Length + 1) / 2;
        for (var index = 0; index < fields.Length; index++)
        {
            var field = fields[index];
            var cell = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, index % 2 == 0 ? 18 : 0, 12) };
            var label = new Label
            {
                Text = field.Label, Dock = DockStyle.Top, Height = 28,
                Font = new Font("Segoe UI", 10F), ForeColor = AppTheme.Text
            };
            field.Input.Dock = DockStyle.Top;
            field.Input.Font = new Font("Segoe UI", 11F);
            field.Input.AccessibleName = field.Label;
            field.Input.Name = $"field{index}";
            field.Input.TabIndex = index;
            if (readOnly)
            {
                if (field.Input is TextBox textBox)
                    textBox.ReadOnly = true;
                else
                    field.Input.Enabled = false;
            }
            cell.Controls.Add(field.Input);
            cell.Controls.Add(label);
            fieldsLayout.Controls.Add(cell, index % 2, index / 2);
            if (index % 2 == 0)
                fieldsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
        }
        if (readOnly)
        {
            saveButton.Visible = false;
            closeButton.Text = "Close";
            AcceptButton = closeButton;
        }
    }

    public static TextBox TextInput(string value = "") => new()
    {
        Text = value, BorderStyle = BorderStyle.FixedSingle,
        BackColor = AppTheme.Background
    };

    public static ComboBox ChoiceInput(string value, params string[] choices)
    {
        var input = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat,
            BackColor = AppTheme.Background
        };
        input.Items.AddRange(choices);
        input.SelectedItem = value;
        if (input.SelectedIndex < 0 && input.Items.Count > 0)
            input.SelectedIndex = 0;
        return input;
    }

    internal Func<Task>? SaveAsync { get; set; }

    private async void SaveButton_Click(object? sender, EventArgs e)
    {
        if (SaveAsync is null)
        {
            MessageBox.Show(this, "Database integration is pending for this screen.",
                "Travel Agency", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (await UiOperation.RunAsync(this, SaveAsync))
            DialogResult = DialogResult.OK;
    }
}

