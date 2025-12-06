using JetBrains.Annotations;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Domain.Reservations;

public record ReservationId(Guid Value);

public class Reservation
{
    private List<ReservationSeat> _reservedSeats = [];

    public Reservation(ReservationId id, Guid eventId, Guid userId, IEnumerable<Guid> seatsIds)
    {
        Id = id;
        EventId = eventId;
        UserId = userId;
        Status = ReservationStatus.Pending;
        CreatedAt = DateTime.UtcNow;

        var reservedSeats = seatsIds
            .Select(seatId => new ReservationSeat(new ReservationSeatId(Guid.NewGuid()), this, new SeatId(seatId)))
            .ToList();
        _reservedSeats = reservedSeats;
    }

    public ReservationId Id { get; private set; } = null!;
    public Guid EventId { get; private set; }
    public Guid UserId { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyList<ReservationSeat> ReservedSeats => _reservedSeats;

    [UsedImplicitly]
    private Reservation()
    {
        // EF Core
    }
}