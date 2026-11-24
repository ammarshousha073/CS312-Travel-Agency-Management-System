#nullable enable

namespace TravelAgencyManagementSystem.Forms;

partial class TripDialog
{
    private System.ComponentModel.IContainer? components;
    private Label titleLabel = null!;
    private Label subtitleLabel = null!;
    private Label destinationLabel = null!;
    private ComboBox destinationComboBox = null!;
    private Label departureLabel = null!;
    private DateTimePicker departureDatePicker = null!;
    private Label durationLabel = null!;
    private NumericUpDown durationNumeric = null!;
    private Label priceLabel = null!;
    private NumericUpDown priceNumeric = null!;
    private Label seatsLabel = null!;
    private NumericUpDown seatsNumeric = null!;
    private Label statusLabel = null!;
    private ComboBox statusComboBox = null!;
    private Label descriptionLabel = null!;
    private TextBox descriptionTextBox = null!;
    private Label previewLabel = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        titleLabel = new Label();
        subtitleLabel = new Label();
        destinationLabel = new Label();
        destinationComboBox = new ComboBox();
        departureLabel = new Label();
        departureDatePicker = new DateTimePicker();
        durationLabel = new Label();
        durationNumeric = new NumericUpDown();
        priceLabel = new Label();
        priceNumeric = new NumericUpDown();
        seatsLabel = new Label();
        seatsNumeric = new NumericUpDown();
        statusLabel = new Label();
        statusComboBox = new ComboBox();
        descriptionLabel = new Label();
        descriptionTextBox = new TextBox();
        previewLabel = new Label();
        saveButton = new Button();
        cancelButton = new Button();
        ((System.ComponentModel.ISupportInitialize)durationNumeric).BeginInit();
        ((System.ComponentModel.ISupportInitialize)priceNumeric).BeginInit();
        ((System.ComponentModel.ISupportInitialize)seatsNumeric).BeginInit();
        SuspendLayout();

        titleLabel.AutoSize = true;
        titleLabel.Font = new Font("Segoe UI", 23F, FontStyle.Bold);
        titleLabel.ForeColor = Color.FromArgb(23, 43, 77);
        titleLabel.Location = new Point(26, 22);
        titleLabel.Text = "Add trip";
        subtitleLabel.Location = new Point(30, 76);
        subtitleLabel.Size = new Size(580, 28);
        subtitleLabel.ForeColor = Color.FromArgb(113, 128, 150);
        subtitleLabel.Text = "Plan a new destination and departure.";

        destinationLabel.Location = new Point(30, 126);
        destinationLabel.Size = new Size(280, 26);
        destinationLabel.Text = "Destination";
        destinationComboBox.Location = new Point(30, 156);
        destinationComboBox.Size = new Size(280, 30);
        destinationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        destinationComboBox.FlatStyle = FlatStyle.Flat;
        destinationComboBox.BackColor = Color.FromArgb(244, 247, 250);
        destinationComboBox.AccessibleName = "Trip destination";
        destinationComboBox.TabIndex = 0;
        destinationComboBox.Name = "destinationComboBox";
        departureLabel.Location = new Point(330, 126);
        departureLabel.Size = new Size(280, 26);
        departureLabel.Text = "Departure date";
        departureDatePicker.Location = new Point(330, 156);
        departureDatePicker.Size = new Size(280, 30);
        departureDatePicker.Format = DateTimePickerFormat.Custom;
        departureDatePicker.CustomFormat = "dd MMM yyyy";
        departureDatePicker.AccessibleName = "Departure date";
        departureDatePicker.TabIndex = 1;
        departureDatePicker.Name = "departureDatePicker";

        durationLabel.Location = new Point(30, 216);
        durationLabel.Size = new Size(280, 26);
        durationLabel.Text = "Duration (days)";
        durationNumeric.Location = new Point(30, 246);
        durationNumeric.Size = new Size(280, 30);
        durationNumeric.Minimum = 1;
        durationNumeric.Maximum = 365;
        durationNumeric.Value = 3;
        durationNumeric.AccessibleName = "Duration in days";
        durationNumeric.TabIndex = 2;
        durationNumeric.Name = "durationNumeric";
        priceLabel.Location = new Point(330, 216);
        priceLabel.Size = new Size(280, 26);
        priceLabel.Text = "Price per traveler (EGP)";
        priceNumeric.Location = new Point(330, 246);
        priceNumeric.Size = new Size(280, 30);
        priceNumeric.Maximum = 1000000;
        priceNumeric.DecimalPlaces = 2;
        priceNumeric.ThousandsSeparator = true;
        priceNumeric.Increment = 100;
        priceNumeric.Value = 4500;
        priceNumeric.AccessibleName = "Price in Egyptian pounds";
        priceNumeric.TabIndex = 3;
        priceNumeric.Name = "priceNumeric";

        seatsLabel.Location = new Point(30, 306);
        seatsLabel.Size = new Size(280, 26);
        seatsLabel.Text = "Total seats";
        seatsNumeric.Location = new Point(30, 336);
        seatsNumeric.Size = new Size(280, 30);
        seatsNumeric.Minimum = 1;
        seatsNumeric.Maximum = 1000;
        seatsNumeric.Value = 20;
        seatsNumeric.AccessibleName = "Total seats";
        seatsNumeric.TabIndex = 4;
        seatsNumeric.Name = "seatsNumeric";
        statusLabel.Location = new Point(330, 306);
        statusLabel.Size = new Size(280, 26);
        statusLabel.Text = "Trip status";
        statusComboBox.Location = new Point(330, 336);
        statusComboBox.Size = new Size(280, 30);
        statusComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        statusComboBox.FlatStyle = FlatStyle.Flat;
        statusComboBox.BackColor = Color.FromArgb(244, 247, 250);
        statusComboBox.AccessibleName = "Trip status";
        statusComboBox.TabIndex = 5;
        statusComboBox.Name = "statusComboBox";

        descriptionLabel.Location = new Point(30, 396);
        descriptionLabel.Size = new Size(580, 26);
        descriptionLabel.Text = "Description";
        descriptionTextBox.Location = new Point(30, 426);
        descriptionTextBox.Size = new Size(580, 64);
        descriptionTextBox.Multiline = true;
        descriptionTextBox.BorderStyle = BorderStyle.FixedSingle;
        descriptionTextBox.ScrollBars = ScrollBars.Vertical;
        descriptionTextBox.PlaceholderText = "Itinerary, accommodation, and trip highlights...";
        descriptionTextBox.AccessibleName = "Trip description";
        descriptionTextBox.TabIndex = 6;
        descriptionTextBox.Name = "descriptionTextBox";
        previewLabel.Location = new Point(30, 510);
        previewLabel.Size = new Size(580, 24);
        previewLabel.Font = new Font("Segoe UI", 9F);
        previewLabel.ForeColor = Color.FromArgb(113, 128, 150);
        previewLabel.Text = "GUI preview · Changes are not saved";
        saveButton.Location = new Point(468, 552);
        saveButton.Size = new Size(142, 42);
        saveButton.Text = "Save preview";
        saveButton.Click += SaveButton_Click;
        saveButton.TabIndex = 7;
        saveButton.Name = "saveButton";
        cancelButton.Location = new Point(330, 552);
        cancelButton.Size = new Size(122, 42);
        cancelButton.Text = "Cancel";
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.TabIndex = 8;
        cancelButton.Name = "cancelButton";

        Controls.Add(titleLabel);
        Controls.Add(subtitleLabel);
        Controls.Add(destinationLabel);
        Controls.Add(destinationComboBox);
        Controls.Add(departureLabel);
        Controls.Add(departureDatePicker);
        Controls.Add(durationLabel);
        Controls.Add(durationNumeric);
        Controls.Add(priceLabel);
        Controls.Add(priceNumeric);
        Controls.Add(seatsLabel);
        Controls.Add(seatsNumeric);
        Controls.Add(statusLabel);
        Controls.Add(statusComboBox);
        Controls.Add(descriptionLabel);
        Controls.Add(descriptionTextBox);
        Controls.Add(previewLabel);
        Controls.Add(saveButton);
        Controls.Add(cancelButton);
        AcceptButton = saveButton;
        CancelButton = cancelButton;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.White;
        ForeColor = Color.FromArgb(38, 52, 69);
        Font = new Font("Segoe UI", 11F);
        ClientSize = new Size(640, 624);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Name = "TripDialog";
        ((System.ComponentModel.ISupportInitialize)durationNumeric).EndInit();
        ((System.ComponentModel.ISupportInitialize)priceNumeric).EndInit();
        ((System.ComponentModel.ISupportInitialize)seatsNumeric).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
