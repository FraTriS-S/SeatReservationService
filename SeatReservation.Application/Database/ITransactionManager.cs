using System.Data;
using CSharpFunctionalExtensions;
using Shared;

namespace SeatReservation.Application.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken cancellationToken = default, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken = default);
}