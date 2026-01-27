using JetBrains.Annotations;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Domain.Reservations;

public record ReservationSeatId(Guid Value);

public class ReservationSeat
{
    public ReservationSeat(ReservationSeatId id, Reservation reservation, SeatId seatId, Guid eventId)
    {
        Id = id;
        Reservation = reservation;
        SeatId = seatId;
        EventId = eventId;
        ReservedAt = DateTime.UtcNow;
    }

    public ReservationSeatId Id { get; } = null!;
    public Reservation Reservation { get; private set; } = null!;
    public SeatId SeatId { get; private set; } = null!;
    public Guid EventId { get; private set; }
    public DateTime ReservedAt { get; }

    [UsedImplicitly]
    private ReservationSeat()
    {
        // EF Core
    }
}