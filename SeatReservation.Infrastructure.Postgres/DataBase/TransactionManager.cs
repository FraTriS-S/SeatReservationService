using System.Data;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SeatReservation.Application.Database;
using Shared;

namespace SeatReservation.Infrastructure.Postgres.DataBase;

public class TransactionManager : ITransactionManager
{
    private readonly ILogger<TransactionManager> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly SeatReservationDbContext _context;

    public TransactionManager(ILogger<TransactionManager> logger, ILoggerFactory loggerFactory, SeatReservationDbContext context)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
        _context = context;
    }

    public async Task<Result<ITransactionScope, Error>> BeginTransactionAsync(
        CancellationToken cancellationToken = default, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        try
        {
            var transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);

            var transactionScopeLogger = _loggerFactory.CreateLogger<TransactionScope>();

            return new TransactionScope(transactionScopeLogger, transaction.GetDbTransaction());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to begin transaction");
            return Error.Failure("database", "Failed to begin transaction");
        }
    }

    public async Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save changes");
            return Error.Failure("save", "Failed to save changes");
        }
    }
}