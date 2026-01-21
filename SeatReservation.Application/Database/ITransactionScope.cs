using CSharpFunctionalExtensions;
using Shared;

namespace SeatReservation.Application.Database;

public interface ITransactionScope : IDisposable
{
    UnitResult<Error> Commit();
    UnitResult<Error> Rollback();
}