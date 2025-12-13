using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using SeatReservation.Application;
using SeatReservation.Domain.Venues;
using SeatReservation.Infrastructure.Postgres.DataBase;
using Shared;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class NpgSqlVenuesRepository : IVenuesRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<NpgSqlVenuesRepository> _logger;

    public NpgSqlVenuesRepository(IDbConnectionFactory connectionFactory, ILogger<NpgSqlVenuesRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> Add(Venue venue, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        using var transaction = connection.BeginTransaction();

        try
        {
            const string venueInsertSql =
                """
                INSERT INTO venues (id, prefix, name, seats_limit)
                VALUES (@Id, @Prefix, @Name, @SeatsLimit)
                """;

            var venueInsertParams = new
            {
                Id = venue.Id.Value,
                Prefix = venue.Name.Prefix,
                Name = venue.Name.Name,
                SeatsLimit = venue.SeatsLimit
            };

            await connection.ExecuteAsync(venueInsertSql, venueInsertParams);

            if (!venue.Seats.Any())
            {
                return venue.Id.Value;
            }

            const string seatsInsertSql =
                """
                INSERT INTO seats (id, row_number, seat_number, venue_id)
                VALUES (@Id, @RowNumber, @SeatNumber, @VenueId)
                """;

            var seatsInsertParams = venue.Seats.Select(seat => new
            {
                Id = seat.Id.Value,
                RowNumber = seat.RowNumber,
                SeatNumber = seat.SeatNumber,
                VenueId = seat.Venue.Id.Value,
            });

            await connection.ExecuteAsync(seatsInsertSql, seatsInsertParams);

            transaction.Commit();

            return venue.Id.Value;
        }
        catch (Exception exception)
        {
            transaction.Rollback();

            _logger.LogError(exception, "Failed to insert venue");

            return Error.Failure("venue.insert", "Failed to insert venue");
        }
    }
}