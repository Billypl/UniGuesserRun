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

        string connectionString;
        if (File.Exists("/.dockerenv"))
            connectionString = configuration.GetConnectionString("PostgreSql");
        else
            connectionString = configuration.GetConnectionString("PostgreSqlLocal");


        Console.WriteLine("######## DEBUG ########");
        Console.WriteLine($"Using connection string: {connectionString}");


        Console.WriteLine($"CurrentDirectory: {Directory.GetCurrentDirectory()}");
        Console.WriteLine($"AppContext.BaseDirectory: {AppContext.BaseDirectory}");


        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        Console.WriteLine($"Checking path: {configPath}");
        Console.WriteLine(File.Exists(configPath) ? "File exists" : "File missing!");


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