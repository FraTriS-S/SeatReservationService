using SeatReservation.Domain.Venues;

namespace SeatReservation.Application.Seats;

public interface ISeatsRepository
{
    Task<IReadOnlyList<Seat>> GetByIdsAsync(IEnumerable<SeatId> seatIds, CancellationToken cancellationToken = default);
}