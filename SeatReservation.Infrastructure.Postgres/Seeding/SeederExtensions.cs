using Microsoft.Extensions.DependencyInjection;

namespace SeatReservation.Infrastructure.Postgres.Seeding;

public static class SeederExtensions
{
    public static async Task<IServiceProvider> RunSeedingAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var seeders = scope.ServiceProvider.GetServices<ISeeder>();

        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync();
        }

        return services;
    }
}