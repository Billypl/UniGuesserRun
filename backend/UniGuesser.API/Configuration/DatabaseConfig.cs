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

        // Select connection string depending on runtime environment.
        // - When running inside Docker, use 'PostgreSql' (service name 'db').
        // - When running locally (developer machine), use 'PostgreSqlLocal' (localhost:5432 - Docker database).
        var isInDocker = File.Exists("/.dockerenv") || string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase);
        var connectionString = isInDocker
            ? configuration.GetConnectionString("PostgreSql")
            : configuration.GetConnectionString("PostgreSqlLocal");

        Console.WriteLine("######## DATABASE CONNECTION ########");
        Console.WriteLine($"Using connection string: {connectionString}");
        Console.WriteLine($"Environment: {(isInDocker ? "Docker (using service name 'db')" : "Local (connecting to localhost:5432 - Docker database)")}");

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