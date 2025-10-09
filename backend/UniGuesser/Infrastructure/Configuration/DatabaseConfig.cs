using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using UniGuesser.Domain.Entities;

namespace UniGuesser.Infrastructure.Configuration
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

            services.AddScoped<GameDbContext>();
            return services;
        }

    }
}
