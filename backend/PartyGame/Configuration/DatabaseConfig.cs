using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using PartyGame.Entities;

namespace PartyGame.DependencyInjections
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
            }, ServiceLifetime.Scoped);

            services.AddScoped<GameDbContext>();
            return services;
        }

    }
}
