using CSharpFunctionalExtensions;
using Shared;

namespace SeatReservation.Application.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken = default);
}