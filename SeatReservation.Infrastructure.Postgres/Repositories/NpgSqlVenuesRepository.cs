// using CSharpFunctionalExtensions;
// using Dapper;
// using Microsoft.Extensions.Logging;
// using SeatReservation.Application;
// using SeatReservation.Domain.Venues;
// using SeatReservation.Infrastructure.Postgres.DataBase;
// using Shared;
//
// namespace SeatReservation.Infrastructure.Postgres.Repositories;
//
// public class NpgSqlVenuesRepository : IVenuesRepository
// {
//     private readonly IDbConnectionFactory _connectionFactory;
//     private readonly ILogger<NpgSqlVenuesRepository> _logger;
//
//     public NpgSqlVenuesRepository(IDbConnectionFactory connectionFactory, ILogger<NpgSqlVenuesRepository> logger)
//     {
//         _connectionFactory = connectionFactory;
//         _logger = logger;
//     }
//
//     public Task<Result<Venue, Error>> GetByIdAsync(VenueId venueId, CancellationToken cancellationToken = default)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<IReadOnlyList<Venue>> GetByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
//     {
//         throw new NotImplementedException();
//     }
//
//     public async Task<Result<Guid, Error>> Add(Venue venue, CancellationToken cancellationToken = default)
//     {
//         using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
//
//         using var transaction = connection.BeginTransaction();
//
//         try
//         {
//             const string venueInsertSql =
//                 """
//                 INSERT INTO venues (id, prefix, name, seats_limit)
//                 VALUES (@Id, @Prefix, @Name, @SeatsLimit)
//                 """;
//
//             var venueInsertParams = new
//             {
//                 Id = venue.Id.Value,
//                 Prefix = venue.Name.Prefix,
//                 Name = venue.Name.Name,
//                 SeatsLimit = venue.SeatsLimit
//             };
//
//             await connection.ExecuteAsync(venueInsertSql, venueInsertParams);
//
//             if (!venue.Seats.Any())
//             {
//                 return venue.Id.Value;
//             }
//
//             const string seatsInsertSql =
//                 """
//                 INSERT INTO seats (id, row_number, seat_number, venue_id)
//                 VALUES (@Id, @RowNumber, @SeatNumber, @VenueId)
//                 """;
//
//             var seatsInsertParams = venue.Seats.Select(seat => new
//             {
//                 Id = seat.Id.Value,
//                 RowNumber = seat.RowNumber,
//                 SeatNumber = seat.SeatNumber,
//                 VenueId = seat.Venue.Id.Value,
//             });
//
//             await connection.ExecuteAsync(seatsInsertSql, seatsInsertParams);
//
//             transaction.Commit();
//
//             return venue.Id.Value;
//         }
//         catch (Exception exception)
//         {
//             transaction.Rollback();
//
//             _logger.LogError(exception, "Failed to insert venue");
//
//             return Error.Failure("venue.insert", "Failed to insert venue");
//         }
//     }
//
//     public async Task<Result<Guid,Error>> UpdateVenueName(VenueId id, VenueName name, CancellationToken cancellationToken = default)
//     {
//         using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
//
//         using var transaction = connection.BeginTransaction();
//
//         try
//         {
//             const string updateNameSql =
//                 """
//                 UPDATE venues
//                 SET name = @Name
//                 WHERE id = @Id
//                 """;
//
//             var updateNameParams = new
//             {
//                 Id = id.Value,
//                 Name = name.Name
//             };
//
//             await connection.ExecuteAsync(updateNameSql, updateNameParams);
//             
//             transaction.Commit();
//             
//             return id.Value;
//         }
//         catch (Exception exception)
//         {
//             transaction.Rollback();
//
//             _logger.LogError(exception, "Failed to update venue");
//
//             return Error.Failure("venue.insert", "Failed to insert venue");
//         }
//     }
//
//     public async Task<UnitResult<Error>> UpdateVenueNameByPrefix(string prefix, VenueName name, CancellationToken cancellationToken = default)
//     {
//         using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
//
//         using var transaction = connection.BeginTransaction();
//
//         try
//         {
//             const string updateNameSql =
//                 """
//                 UPDATE venues
//                 SET name = @Name
//                 WHERE prefix LIKE @Prefix
//                 """;
//
//             var updateNameParams = new
//             {
//                 prefix = $"{prefix}%",
//                 Name = name.Name
//             };
//
//             await connection.ExecuteAsync(updateNameSql, updateNameParams);
//             
//             transaction.Commit();
//             
//             return UnitResult.Success<Error>();
//         }
//         catch (Exception exception)
//         {
//             transaction.Rollback();
//
//             _logger.LogError(exception, "Failed to update venue");
//
//             return Error.Failure("venue.insert", "Failed to insert venue");
//         }
//     }
//
//     public Task<UnitResult<Error>> DeleteSeatsByVenueIdAsync(VenueId venueId, CancellationToken cancellationToken = default)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<UnitResult<Error>> AddSeatsAsync(IEnumerable<Seat> seats, CancellationToken cancellationToken = default)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task SaveAsync(CancellationToken cancellationToken = default)
//     {
//         throw new NotImplementedException();
//     }
// }