using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using SeatReservation.Domain.Venues;
using Shared;

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
            .Select(seatId => new ReservationSeat(new ReservationSeatId(Guid.NewGuid()), this, new SeatId(seatId), eventId))
            .ToList();
        _reservedSeats = reservedSeats;
    }

    public ReservationId Id { get; private set; } = null!;
    public Guid EventId { get; private set; }
    public Guid UserId { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyList<ReservationSeat> ReservedSeats => _reservedSeats;

    public static Result<Reservation, Error> Create(Guid eventId, Guid userId, IEnumerable<Guid> seatsIds)
    {
        if (eventId == Guid.Empty)
        {
            return Error.Validation("reservation.eventId", "EventId cannot be empty");
        }

        if (userId == Guid.Empty)
        {
            return Error.Validation("reservation.userId", "UserId cannot be empty");
        }

        var reservationId = new ReservationId(Guid.NewGuid());

        return new Reservation(reservationId, eventId, userId, seatsIds);
    }

    [UsedImplicitly]
    private Reservation()
    {
        // EF Core
    }
}