using TravelAgencyManagementSystem.Data;
using TravelAgencyManagementSystem.Styling;

namespace TravelAgencyManagementSystem.Forms;

public partial class TripDialog : Form
{
    internal Func<Trip, Task>? SaveAsync { get; set; }
    private readonly Trip? original;

    public TripDialog() : this("Add trip", null, false)
    {
    }

    internal TripDialog(string title, Trip? trip, bool readOnly)
    {
        original = trip;
        InitializeComponent();
        Text = title;
        titleLabel.Text = title;
        destinationComboBox.Items.AddRange(["Cairo", "Luxor", "Sharm El Sheikh", "Aswan", "Hurghada"]);
        destinationComboBox.DropDownStyle = ComboBoxStyle.DropDown;
        destinationComboBox.Text = trip?.Destination ?? "Cairo";
        statusComboBox.Items.AddRange(["Open", "Full", "Cancelled"]);
        statusComboBox.SelectedItem = trip?.Status ?? "Open";
        departureDatePicker.Value = trip?.DepartureDate ?? DateTime.Today.AddDays(1);
        priceNumeric.DecimalPlaces = 2;
        priceNumeric.Minimum = 0;
        durationNumeric.Maximum = int.MaxValue;
        seatsNumeric.Maximum = int.MaxValue;
        priceNumeric.Maximum = 1_000_000_000;
        if (trip is not null)
        {
            subtitleLabel.Text = $"{trip.Capacity - trip.AvailableSeats} / {trip.Capacity} seats booked";
            durationNumeric.Value = trip.Duration;
            priceNumeric.Value = trip.Price;
            seatsNumeric.Value = trip.Capacity;
            descriptionTextBox.Text = trip.Description;
        }
        else
        {
            subtitleLabel.Text = "Enter trip details";
        }
        previewLabel.Visible = false;
        saveButton.Text = "Save";
        AppTheme.StylePrimaryButton(saveButton);
        AppTheme.StyleSecondaryButton(cancelButton);
        if (readOnly)
        {
            destinationComboBox.Enabled = false;
            departureDatePicker.Enabled = false;
            durationNumeric.Enabled = false;
            priceNumeric.Enabled = false;
            seatsNumeric.Enabled = false;
            statusComboBox.Enabled = false;
            descriptionTextBox.ReadOnly = true;
            saveButton.Visible = false;
            cancelButton.Text = "Close";
            AcceptButton = cancelButton;
        }
    }

    private async void SaveButton_Click(object? sender, EventArgs e)
    {
        if (SaveAsync is null)
            return;
        var trip = new Trip
        {
            Id = original?.Id ?? default,
            UpdatedAt = original?.UpdatedAt ?? DateTime.UtcNow,
            Destination = destinationComboBox.Text,
            DepartureDate = DateTime.SpecifyKind(departureDatePicker.Value.Date, DateTimeKind.Utc),
            Duration = (int)durationNumeric.Value,
            Price = priceNumeric.Value,
            Capacity = (int)seatsNumeric.Value,
            Status = statusComboBox.Text,
            Description = descriptionTextBox.Text.Trim()
        };
        if (await UiOperation.RunAsync(this, () => SaveAsync(trip)))
            DialogResult = DialogResult.OK;
    }
}
