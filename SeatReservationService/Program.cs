using SeatReservation.Infrastructure.Postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddScoped<SeatReservationDbContext>(_ =>
    new SeatReservationDbContext(builder.Configuration.GetConnectionString("SeatReservationDb")!));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
app.UseSwaggerUI(options=>
{
    options.SwaggerEndpoint("/openapi/v1.json", "SeatReservationService");
    options.RoutePrefix = "swagger";
});

}

app.Run();