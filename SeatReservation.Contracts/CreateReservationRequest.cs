namespace SeatReservation.Contracts;

public record CreateReservationRequest(Guid EventId, Guid UserId, IEnumerable<Guid> SeatIds);