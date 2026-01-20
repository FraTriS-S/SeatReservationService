using SeatReservation.Application;
using SeatReservation.Application.Venues;
using SeatReservation.Infrastructure.Postgres;
using SeatReservation.Infrastructure.Postgres.DataBase;
using SeatReservation.Infrastructure.Postgres.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddScoped<IDbConnectionFactory, NpgSqlConnectionFactory>();

builder.Services.AddScoped<SeatReservationDbContext>(_ =>
    new SeatReservationDbContext(builder.Configuration.GetConnectionString("SeatReservationDb")!));

builder.Services.AddSingleton<IDbConnectionFactory, NpgSqlConnectionFactory>();

//builder.Services.AddScoped<IVenuesRepository, NpgSqlVenuesRepository>();
builder.Services.AddScoped<IVenuesRepository, EfCoreVenuesRepository>();

builder.Services.AddScoped<CreateVenueHandler>();
builder.Services.AddScoped<UpdateVenueNameHandler>();
builder.Services.AddScoped<UpdateVenueNameByPrefixHandler>();
builder.Services.AddScoped<UpdateVenueSeatsHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "SeatReservationService");
        options.RoutePrefix = "swagger";
    });
}

app.MapControllers();

app.Run();