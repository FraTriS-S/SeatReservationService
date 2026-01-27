using CSharpFunctionalExtensions;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Reservations;

public interface IReservationsRepository
{
    Task<Result<Guid, Error>> AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task<bool> AnySeatsAlreadyReserved(Guid eventId, IEnumerable<SeatId> seatIds, CancellationToken cancellationToken = default);
    Task<int> GetReservedSeatsCount(Guid eventId, CancellationToken cancellationToken = default);
}