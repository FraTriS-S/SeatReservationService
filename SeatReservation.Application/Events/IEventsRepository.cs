using CSharpFunctionalExtensions;
using SeatReservation.Domain.Events;
using Shared;

namespace SeatReservation.Application.Events;

public interface IEventsRepository
{
    Task<Result<Event, Error>> GetByIdAsync(EventId eventId, CancellationToken cancellationToken = default);
}