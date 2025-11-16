using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniGuesser.Infrastructure.Persistence;

namespace UniGuesser.API.Configuration;

public static class DatabaseConfig
{
    public static IServiceCollection AddDatabaseConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        // Always use localhost connection (Docker database on port 5432)
        // Backend running on Windows connects to Docker PostgreSQL container
        var connectionString = configuration.GetConnectionString("PostgreSqlLocal");

        Console.WriteLine("######## DATABASE CONNECTION ########");
        Console.WriteLine($"Using connection string: {connectionString}");
        Console.WriteLine($"Environment: {(File.Exists("/.dockerenv") ? "Docker" : "Local")}");

        services.AddDbContext<GameDbContext>(options => { options.UseNpgsql(connectionString); });

        return services;
    }

    public static void MigrateDatabase(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
            db.Database.Migrate();
        }
    }
}