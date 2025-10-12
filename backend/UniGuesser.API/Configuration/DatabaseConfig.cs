using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniGuesser.Infrastructure.Persistence;

namespace UniGuesser.API.Configuration
{
    public static class DatabaseConfig
    {
        public static IServiceCollection AddDatabaseConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            string connectionString;
            if (File.Exists("/.dockerenv"))
            {
                connectionString = configuration.GetConnectionString("PostgreSql");
            }
            else
            {
                connectionString = configuration.GetConnectionString("PostgreSqlLocal");
            }

            services.AddDbContext<GameDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            }, ServiceLifetime.Singleton);


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
}