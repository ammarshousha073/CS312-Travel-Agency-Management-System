#nullable enable

namespace TravelAgencyManagementSystem.Forms;

partial class RecordDialog
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel dialogLayout = null!;
    private Panel titlePanel = null!;
    private Label titleLabel = null!;
    private Label subtitleLabel = null!;
    private TableLayoutPanel fieldsLayout = null!;
    private Panel footerPanel = null!;
    private Label previewLabel = null!;
    private Button saveButton = null!;
    private Button closeButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        dialogLayout = new TableLayoutPanel();
        titlePanel = new Panel();
        titleLabel = new Label();
        subtitleLabel = new Label();
        fieldsLayout = new TableLayoutPanel();
        footerPanel = new Panel();
        previewLabel = new Label();
        saveButton = new Button();
        closeButton = new Button();
        dialogLayout.SuspendLayout();
        titlePanel.SuspendLayout();
        footerPanel.SuspendLayout();
        SuspendLayout();
        dialogLayout.Size = new Size(700, 620);
        dialogLayout.Dock = DockStyle.Fill;
        dialogLayout.Padding = new Padding(30, 20, 30, 22);
        dialogLayout.ColumnCount = 1;
        dialogLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        dialogLayout.RowCount = 4;
        dialogLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 104F));
        dialogLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        dialogLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        dialogLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
        dialogLayout.Controls.Add(titlePanel, 0, 0);
        dialogLayout.Controls.Add(fieldsLayout, 0, 1);
        dialogLayout.Controls.Add(previewLabel, 0, 2);
        dialogLayout.Controls.Add(footerPanel, 0, 3);
        titlePanel.Dock = DockStyle.Fill;
        titlePanel.Controls.Add(subtitleLabel);
        titlePanel.Controls.Add(titleLabel);
        titlePanel.Margin = new Padding(0);
        titleLabel.Dock = DockStyle.Top;
        titleLabel.Height = 54;
        titleLabel.Font = new Font("Segoe UI", 23F, FontStyle.Bold);
        titleLabel.ForeColor = Color.FromArgb(23, 43, 77);
        titleLabel.Text = "Record preview";
        subtitleLabel.Dock = DockStyle.Fill;
        subtitleLabel.ForeColor = Color.FromArgb(113, 128, 150);
        subtitleLabel.Text = "Sample record";
        fieldsLayout.Dock = DockStyle.Fill;
        fieldsLayout.AutoScroll = true;
        fieldsLayout.ColumnCount = 2;
        fieldsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        fieldsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        fieldsLayout.Margin = new Padding(0);
        fieldsLayout.Name = "fieldsLayout";
        previewLabel.Dock = DockStyle.Fill;
        previewLabel.Font = new Font("Segoe UI", 9F);
        previewLabel.ForeColor = Color.FromArgb(113, 128, 150);
        previewLabel.Text = "GUI preview · Changes are not saved";
        previewLabel.Margin = new Padding(0);
        footerPanel.Size = new Size(640, 52);
        footerPanel.Dock = DockStyle.Fill;
        footerPanel.Margin = new Padding(0);
        footerPanel.Controls.Add(saveButton);
        footerPanel.Controls.Add(closeButton);
        saveButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        saveButton.Location = new Point(492, 8);
        saveButton.Size = new Size(148, 42);
        saveButton.Text = "Save preview";
        saveButton.Name = "saveButton";
        saveButton.Click += SaveButton_Click;
        closeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        closeButton.Location = new Point(354, 8);
        closeButton.Size = new Size(122, 42);
        closeButton.Text = "Cancel";
        closeButton.DialogResult = DialogResult.Cancel;
        closeButton.Name = "closeButton";
        Controls.Add(dialogLayout);
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(700, 620);
        Font = new Font("Segoe UI", 10F);
        ForeColor = Color.FromArgb(38, 52, 69);
        BackColor = Color.White;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        AcceptButton = saveButton;
        CancelButton = closeButton;
        Name = "RecordDialog";
        Text = "Record preview";
        dialogLayout.ResumeLayout(false);
        titlePanel.ResumeLayout(false);
        footerPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
