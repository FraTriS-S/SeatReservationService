namespace SeatReservation.Contracts.Reservations;

public record CreateReservationRequest(Guid EventId, Guid UserId, IEnumerable<Guid> SeatIds);