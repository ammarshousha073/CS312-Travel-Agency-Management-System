using MongoDB.Driver;

namespace TravelAgencyManagementSystem.Data;

internal sealed record ReportMonth(DateTime Month, decimal Revenue, int Bookings);
internal sealed record ReportDestination(string Name, int Bookings);
internal sealed record AgencyReport(decimal Revenue, int Bookings, int Clients, ReportMonth[] Months, ReportDestination[] Destinations);

internal sealed partial class AgencyStore
{
    public async Task<AgencyReport> GenerateReportAsync(DateTime from, DateTime to)
    {
        var start = DateTime.SpecifyKind(from.Date, DateTimeKind.Utc);
        var last = DateTime.SpecifyKind(to.Date, DateTimeKind.Utc);
        if (last < start || last == DateTime.MaxValue.Date)
            throw new ArgumentException("Select a valid date range with the start on or before the end.");
        var end = last.AddDays(1);
        var paymentStart = DateTime.SpecifyKind(start, DateTimeKind.Local).ToUniversalTime();
        var paymentEnd = DateTime.SpecifyKind(end, DateTimeKind.Local).ToUniversalTime();

        await RecoverReservationsAsync();
        var bookingsTask = Bookings.Find(x => x.BookingDate >= start && x.BookingDate < end)
            .Project(x => new { x.BookingDate, x.ClientId, x.TripId }).ToListAsync();
        var paymentsTask = Payments.Find(x => x.PaymentDate >= paymentStart && x.PaymentDate < paymentEnd &&
            (x.Status == "Paid" || x.Status == "Partial")).Project(x => new { x.PaymentDate, x.Amount }).ToListAsync();
        await Task.WhenAll(bookingsTask, paymentsTask);
        var bookings = await bookingsTask;
        var payments = await paymentsTask;
        var tripIds = bookings.Select(x => x.TripId).Distinct().ToArray();
        var trips = tripIds.Length == 0 ? [] : await Trips.Find(x => tripIds.Contains(x.Id))
            .Project(x => new { x.Id, x.Destination }).ToListAsync();
        var names = trips.ToDictionary(x => x.Id, x => x.Destination);

        var bookedByMonth = bookings.ToLookup(x => (x.BookingDate.Year, x.BookingDate.Month));
        var paidByMonth = payments.ToLookup(x =>
        {
            var local = x.PaymentDate.ToLocalTime();
            return (local.Year, local.Month);
        });
        var firstMonth = new DateTime(start.Year, start.Month, 1);
        var monthCount = (last.Year - start.Year) * 12 + last.Month - start.Month + 1;
        var months = Enumerable.Range(0, monthCount).Select(offset =>
        {
            var month = firstMonth.AddMonths(offset);
            var key = (month.Year, month.Month);
            return new ReportMonth(month, paidByMonth[key].Sum(x => x.Amount), bookedByMonth[key].Count());
        }).ToArray();

        var destinations = bookings.GroupBy(x => names.GetValueOrDefault(x.TripId, "Unknown trip"),
                StringComparer.OrdinalIgnoreCase)
            .Select(x => new ReportDestination(x.Key, x.Count()))
            .OrderByDescending(x => x.Bookings).ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        return new AgencyReport(payments.Sum(x => x.Amount), bookings.Count,
            bookings.Select(x => x.ClientId).Distinct().Count(), months, destinations);
    }
}
