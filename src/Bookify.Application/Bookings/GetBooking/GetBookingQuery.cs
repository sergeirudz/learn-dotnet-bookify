using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Bookings.GetBooking;

public sealed record GetBookingQuery(Guid BookingId) : IQuery<BookingResponse>
{
    public string CacheKey => $"bookings-{BookingId}";

    public TimeSpan? Expiration => null;
}
