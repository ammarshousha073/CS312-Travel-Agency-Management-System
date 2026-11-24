using MongoDB.Bson;
using MongoDB.Driver;

namespace TravelAgencyManagementSystem.Data;

internal sealed partial class AgencyStore
{
    public static IReadOnlyList<string> PaymentMethods { get; } = Array.AsReadOnly(new[] { "Cash", "Card", "Bank transfer" });

    public async Task<Booking> CreateBookingAsync(ObjectId actor, ObjectId requestId, ObjectId clientId, ObjectId tripId,
        DateTime date, int travelers, string requests)
    {
        await RequireManagementAsync(actor);
        if (requestId == ObjectId.Empty || travelers <= 0)
            throw new ArgumentException("Enter a positive traveler count.");
        date = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        requests = requests.Trim();

        await RecoverSeatChangesAsync();
        var previous = await Bookings.Find(x => x.Id == requestId).FirstOrDefaultAsync();
        if (previous is not null)
            return VerifyBookingRequest(previous, clientId, tripId, date, travelers, requests);

        if (!await Clients.Find(x => x.Id == clientId && x.Status == "Active").AnyAsync())
            throw new ArgumentException("Select an active client.");
        var trip = await Trips.Find(x => x.Id == tripId).FirstOrDefaultAsync()
            ?? throw new ArgumentException("Select an existing trip.");
        if (trip.Status != "Open" || trip.DepartureDate.Date < DateTime.Today || trip.AvailableSeats < travelers)
            throw new InvalidOperationException("The trip is unavailable or has insufficient seats.");
        if (date > trip.DepartureDate.Date)
            throw new ArgumentException("The booking date must be on or before departure.");

        var total = trip.Price * travelers;
        var booking = new Booking
        {
            Id = requestId, BookingNumber = $"BK-{requestId}", ClientId = clientId, TripId = tripId,
            BookingDate = date, Travelers = travelers, TotalAmount = total, RemainingAmount = total,
            Status = total == 0 ? "Confirmed" : "Pending", SpecialRequests = requests
        };

        booking.SeatOperationId = ObjectId.GenerateNewId();
        var work = new SeatChange { OperationId = booking.SeatOperationId, Booking = booking };
        var staged = await Trips.UpdateOneAsync(x => x.Id == tripId && x.PendingSeats == null &&
            x.Status == "Open" && x.AvailableSeats >= travelers && x.Price == trip.Price &&
            x.DepartureDate == trip.DepartureDate, Builders<Trip>.Update.Set(x => x.PendingSeats, work));
        if (staged.MatchedCount == 0)
            throw new InvalidOperationException("The trip changed while booking. Refresh and retry.");

        await CompleteSeatChangeAsync(tripId, work);
        return VerifyBookingRequest(await Bookings.Find(x => x.Id == requestId).FirstAsync(),
            clientId, tripId, date, travelers, requests);
    }

    private static Booking VerifyBookingRequest(Booking booking, ObjectId clientId, ObjectId tripId,
        DateTime date, int travelers, string requests)
    {
        if (booking.ClientId != clientId || booking.TripId != tripId || booking.BookingDate != date ||
            booking.Travelers != travelers || booking.SpecialRequests != requests)
            throw new InvalidOperationException("This request already saved a booking. Reopen the list to review it.");
        return booking;
    }

    public async Task CancelBookingAsync(ObjectId actor, ObjectId bookingId)
    {
        await RequireManagementAsync(actor);
        await RecoverReservationsAsync();
        var booking = await Bookings.Find(x => x.Id == bookingId).FirstOrDefaultAsync()
            ?? throw new ArgumentException("The booking no longer exists.");
        if (booking.Status == "Cancelled")
            return;
        if (booking.PaidAmount != 0)
            throw new InvalidOperationException("This booking has recorded payments and cannot be cancelled here.");

        var work = new SeatChange { OperationId = ObjectId.GenerateNewId(), Booking = booking, Cancel = true };
        var staged = await Trips.UpdateOneAsync(x => x.Id == booking.TripId && x.PendingSeats == null &&
            x.AvailableSeats <= x.Capacity - booking.Travelers, Builders<Trip>.Update.Set(x => x.PendingSeats, work));
        if (staged.MatchedCount == 0)
        {
            await RecoverSeatChangesAsync();
            if (await Bookings.Find(x => x.Id == bookingId && x.Status == "Cancelled").AnyAsync())
                return;
            throw new InvalidOperationException("Could not release the reserved seats. Refresh the booking and retry.");
        }

        await CompleteSeatChangeAsync(booking.TripId, work);
        if (!await Bookings.Find(x => x.Id == bookingId && x.Status == "Cancelled").AnyAsync())
            throw new InvalidOperationException("A payment was recorded before cancellation. The booking remains active.");
    }

    public async Task<Payment> RegisterPaymentAsync(ObjectId actor, ObjectId requestId, ObjectId bookingId,
        decimal amount, string method)
    {
        if (!await Users.Find(x => x.Id == actor && x.Status == "Active" &&
            (x.Role == "Administrator" || x.Role == "Accountant")).AnyAsync())
            throw new InvalidOperationException("Your account cannot register payments.");
        if (requestId == ObjectId.Empty || amount <= 0 || decimal.Round(amount, 2) != amount)
            throw new ArgumentException("Enter a positive payment amount with up to two decimal places.");
        if (!PaymentMethods.Contains(method))
            throw new ArgumentException("Select a supported payment method.");

        await RecoverReservationsAsync();
        var booking = await Bookings.Find(x => x.Id == bookingId).FirstOrDefaultAsync()
            ?? throw new ArgumentException("Select an existing booking.");
        var previous = await Payments.Find(x => x.Id == requestId).FirstOrDefaultAsync();
        if (previous is not null)
            return VerifyPaymentRequest(previous, bookingId, amount, method);
        if (booking.Status == "Cancelled" || booking.RemainingAmount < amount)
            throw new InvalidOperationException("The booking is cancelled or the amount exceeds its remaining balance.");

        var remaining = booking.RemainingAmount - amount;
        var payment = new Payment
        {
            Id = requestId, OperationId = ObjectId.GenerateNewId(), PaymentNumber = $"PM-{requestId}", BookingId = bookingId, ClientId = booking.ClientId,
            Amount = amount, PaymentMethod = method, PaymentDate = DateTime.UtcNow,
            Status = remaining == 0 ? "Paid" : "Partial", ReceiptReference = $"RC-{requestId}"
        };

        // Only one writer can apply the balance change. Its receipt travels with that same atomic update.
        var staged = await Bookings.UpdateOneAsync(x => x.Id == bookingId && x.PendingPayment == null &&
            x.Status != "Cancelled" && x.PaidAmount == booking.PaidAmount && x.RemainingAmount == booking.RemainingAmount,
            Builders<Booking>.Update.Set(x => x.PendingPayment, payment).Inc(x => x.PaidAmount, amount)
                .Set(x => x.RemainingAmount, remaining).Set(x => x.Status, remaining == 0 ? "Confirmed" : "Pending")
                .Set(x => x.UpdatedAt, DateTime.UtcNow));
        if (staged.MatchedCount == 0)
            throw new InvalidOperationException("The booking balance changed. Refresh and retry.");
        await CompletePaymentAsync(payment);
        return await Payments.Find(x => x.Id == requestId).FirstAsync();
    }

    private static Payment VerifyPaymentRequest(Payment payment, ObjectId bookingId, decimal amount, string method)
    {
        if (payment.BookingId != bookingId || payment.Amount != amount || payment.PaymentMethod != method)
            throw new InvalidOperationException("This request already saved a payment. Reopen the list to review it.");
        return payment;
    }

    private async Task RecoverReservationsAsync()
    {
        await RecoverSeatChangesAsync();
        var pending = await Bookings.Find(x => x.PendingPayment != null).ToListAsync();
        foreach (var booking in pending)
            await CompletePaymentAsync(booking.PendingPayment!);
    }

    private async Task RecoverSeatChangesAsync()
    {
        var pending = await Trips.Find(x => x.PendingSeats != null).ToListAsync();
        foreach (var trip in pending)
            await CompleteSeatChangeAsync(trip.Id, trip.PendingSeats!);
    }

    private async Task CompleteSeatChangeAsync(ObjectId tripId, SeatChange work)
    {
        var booking = work.Booking;
        var change = -booking.Travelers;
        if (work.Cancel)
        {
            // Payment staging and cancellation both check the same booking document.
            await Bookings.UpdateOneAsync(x => x.Id == booking.Id && x.Status != "Cancelled" &&
                x.PaidAmount == 0 && x.PendingPayment == null, Builders<Booking>.Update
                    .Set(x => x.Status, "Cancelled").Set(x => x.CancellationId, work.OperationId)
                    .Set(x => x.UpdatedAt, DateTime.UtcNow));
            change = await Bookings.Find(x => x.Id == booking.Id && x.CancellationId == work.OperationId).AnyAsync()
                ? booking.Travelers : 0;
        }
        else
        {
            try
            {
                await Bookings.InsertOneAsync(booking);
            }
            catch (MongoWriteException error) when (error.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                var saved = await Bookings.Find(x => x.Id == booking.Id).FirstOrDefaultAsync();
                if (saved is null || saved.ClientId != booking.ClientId || saved.TripId != tripId ||
                    saved.Travelers != booking.Travelers || saved.BookingDate != booking.BookingDate ||
                    saved.SpecialRequests != booking.SpecialRequests)
                {
                    // No seats have changed yet. Discard a conflicting creation request.
                    await Trips.UpdateOneAsync(x => x.Id == tripId && x.PendingSeats!.OperationId == work.OperationId,
                        Builders<Trip>.Update.Unset(x => x.PendingSeats));
                    throw new InvalidOperationException("The booking request conflicts with an existing record.");
                }
                if (saved.SeatOperationId != work.OperationId)
                    change = 0;
            }
        }

        var trip = await Trips.Find(x => x.Id == tripId && x.PendingSeats!.OperationId == work.OperationId)
            .FirstOrDefaultAsync();
        if (trip is null)
            return;

        var seats = trip.AvailableSeats + change;
        var status = trip.Status == "Cancelled" ? "Cancelled" : seats == 0 ? "Full" : "Open";
        // Clearing this specific marker and applying the seat change are one atomic write.
        await Trips.UpdateOneAsync(x => x.Id == tripId && x.PendingSeats!.OperationId == work.OperationId,
            Builders<Trip>.Update.Set(x => x.AvailableSeats, seats).Set(x => x.Status, status)
                .Set(x => x.UpdatedAt, DateTime.UtcNow).Unset(x => x.PendingSeats));
    }

    private async Task CompletePaymentAsync(Payment payment)
    {
        try
        {
            await Payments.InsertOneAsync(payment);
        }
        catch (MongoWriteException error) when (error.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            var saved = await Payments.Find(x => x.Id == payment.Id).FirstOrDefaultAsync();
            if (saved is null || saved.OperationId != payment.OperationId)
            {
                // A unique receipt conflict is definitive. Reverse only this attempt's tagged balance change.
                await Bookings.UpdateOneAsync(x => x.Id == payment.BookingId &&
                    x.PendingPayment!.Id == payment.Id && x.PendingPayment.OperationId == payment.OperationId,
                    Builders<Booking>.Update.Inc(x => x.PaidAmount, -payment.Amount)
                        .Inc(x => x.RemainingAmount, payment.Amount).Set(x => x.Status, "Pending")
                        .Set(x => x.UpdatedAt, DateTime.UtcNow).Unset(x => x.PendingPayment));
                if (saved is null)
                    throw new InvalidOperationException("The receipt number conflicts with an existing receipt.");
                VerifyPaymentRequest(saved, payment.BookingId, payment.Amount, payment.PaymentMethod);
                return;
            }
        }
        await Bookings.UpdateOneAsync(x => x.Id == payment.BookingId && x.PendingPayment!.Id == payment.Id &&
            x.PendingPayment.OperationId == payment.OperationId,
            Builders<Booking>.Update.Unset(x => x.PendingPayment));
    }

    public async Task<ReservationSnapshot> LoadReservationsAsync()
    {
        await RecoverReservationsAsync();
        var bookingsTask = Bookings.Find(FilterDefinition<Booking>.Empty)
            .SortByDescending(x => x.BookingDate).ThenByDescending(x => x.Id).ToListAsync();
        var paymentsTask = Payments.Find(FilterDefinition<Payment>.Empty).SortByDescending(x => x.PaymentDate).ToListAsync();
        var clientsTask = LoadClientsAsync();
        var tripsTask = Trips.Find(FilterDefinition<Trip>.Empty).ToListAsync();
        await Task.WhenAll(bookingsTask, paymentsTask, clientsTask, tripsTask);

        var clients = clientsTask.Result.ToDictionary(x => x.Id, x => x.FullName);
        var trips = tripsTask.Result.ToDictionary(x => x.Id);
        var numbers = bookingsTask.Result.ToDictionary(x => x.Id, x => x.BookingNumber);
        var bookings = bookingsTask.Result.Select(booking =>
        {
            var trip = trips.GetValueOrDefault(booking.TripId);
            return new BookingRow(booking, clients.GetValueOrDefault(booking.ClientId, "Unknown client"),
                trip?.Destination ?? "Unknown trip", trip?.DepartureDate);
        }).ToList();
        var payments = paymentsTask.Result.Select(payment => new PaymentRow(payment,
            numbers.GetValueOrDefault(payment.BookingId, "Unknown booking"),
            clients.GetValueOrDefault(payment.ClientId, "Unknown client"))).ToList();
        return new ReservationSnapshot(bookings, payments);
    }
}
