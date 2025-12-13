using System.Data;

namespace SeatReservation.Infrastructure.Postgres.DataBase;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}