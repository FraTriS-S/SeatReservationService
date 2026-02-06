using SeatReservation.Application.Database;
using SeatReservation.Application.Events;
using SeatReservation.Application.Events.Queries;
using SeatReservation.Application.Reservations;
using SeatReservation.Application.Reservations.Commands;
using SeatReservation.Application.Seats;
using SeatReservation.Application.Venues;
using SeatReservation.Application.Venues.Commands;
using SeatReservation.Infrastructure.Postgres;
using SeatReservation.Infrastructure.Postgres.DataBase;
using SeatReservation.Infrastructure.Postgres.Repositories;
using SeatReservation.Infrastructure.Postgres.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddScoped<IDbConnectionFactory, NpgSqlConnectionFactory>();

builder.Services.AddScoped<SeatReservationDbContext>(_ =>
    new SeatReservationDbContext(builder.Configuration.GetConnectionString("SeatReservationDb")!));

builder.Services.AddScoped<IReadDbContext, SeatReservationDbContext>(_ =>
    new SeatReservationDbContext(builder.Configuration.GetConnectionString("SeatReservationDb")!));

builder.Services.AddSingleton<IDbConnectionFactory, NpgSqlConnectionFactory>();
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.AddScoped<ITransactionManager, TransactionManager>();

builder.Services.AddScoped<IEventsRepository, EventsRepository>();
builder.Services.AddScoped<IReservationsRepository, ReservationsRepository>();
builder.Services.AddScoped<ISeatsRepository, SeatsRepository>();
builder.Services.AddScoped<IVenuesRepository, VenuesRepository>();

builder.Services.AddScoped<GetEventByIdQueryHandler>();
builder.Services.AddScoped<GetEventByIdDapperQueryHandler>();
builder.Services.AddScoped<GetEventsQueryHandler>();
builder.Services.AddScoped<GetEventsDapperQueryHandler>();

builder.Services.AddScoped<CreateReservationHandler>();
builder.Services.AddScoped<ReserveAdjacentSeatsHandler>();

builder.Services.AddScoped<CreateVenueHandler>();
builder.Services.AddScoped<UpdateVenueNameHandler>();
builder.Services.AddScoped<UpdateVenueNameByPrefixHandler>();
builder.Services.AddScoped<UpdateVenueSeatsHandler>();

builder.Services.AddScoped<ISeeder, ReservationSeeder>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "SeatReservationService");
        options.RoutePrefix = "swagger";
    });

    if (args.Contains("--seeding"))
    {
        await app.Services.RunSeedingAsync();
    }
}

app.MapControllers();

app.Run();