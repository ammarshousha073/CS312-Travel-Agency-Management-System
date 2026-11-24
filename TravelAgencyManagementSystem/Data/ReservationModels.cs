using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TravelAgencyManagementSystem.Data;

[BsonIgnoreExtraElements]
internal sealed class Booking
{
    public ObjectId Id { get; set; }
    public string BookingNumber { get; set; } = "";
    public ObjectId ClientId { get; set; }
    public ObjectId TripId { get; set; }
    public DateTime BookingDate { get; set; }
    public int Travelers { get; set; }
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal TotalAmount { get; set; }
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal PaidAmount { get; set; }
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal RemainingAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public string SpecialRequests { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    [BsonIgnoreIfNull]
    public Payment? PendingPayment { get; set; }
    [BsonIgnoreIfDefault]
    public ObjectId CancellationId { get; set; }
    [BsonIgnoreIfDefault]
    public ObjectId SeatOperationId { get; set; }

    public string PaymentStatus => PaidAmount == 0 ? "Unpaid" : RemainingAmount == 0 ? "Paid" : "Partial";
}

[BsonIgnoreExtraElements]
internal sealed class Payment
{
    public ObjectId Id { get; set; }
    public string PaymentNumber { get; set; } = "";
    public ObjectId BookingId { get; set; }
    public ObjectId ClientId { get; set; }
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "";
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; } = "";
    public string ReceiptReference { get; set; } = "";
    [BsonIgnoreIfDefault]
    public ObjectId OperationId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

internal sealed class SeatChange
{
    public ObjectId OperationId { get; set; }
    public Booking Booking { get; set; } = new();
    public bool Cancel { get; set; }
}

internal sealed record BookingRow(Booking Booking, string Client, string Destination, DateTime? Departure);
internal sealed record PaymentRow(Payment Payment, string BookingNumber, string Client);
internal sealed record ReservationSnapshot(List<BookingRow> Bookings, List<PaymentRow> Payments)
{
    public IEnumerable<Payment> ReceivedPayments => Payments.Select(row => row.Payment)
        .Where(payment => payment.Status is "Paid" or "Partial");
    public decimal Revenue => ReceivedPayments.Sum(payment => payment.Amount);
    public decimal Outstanding => Bookings.Where(row => row.Booking.Status != "Cancelled")
        .Sum(row => row.Booking.RemainingAmount);

    public (string Month, decimal Revenue)[] MonthlyRevenue(DateTime today)
    {
        var lastMonth = new DateTime(today.Year, today.Month, 1);
        var received = ReceivedPayments.ToLookup(payment =>
        {
            var date = payment.PaymentDate.ToLocalTime();
            return (date.Year, date.Month);
        });
        return Enumerable.Range(-5, 6).Select(offset =>
        {
            var month = lastMonth.AddMonths(offset);
            return (month.ToString("MMM"), received[(month.Year, month.Month)].Sum(payment => payment.Amount));
        }).ToArray();
    }
}
