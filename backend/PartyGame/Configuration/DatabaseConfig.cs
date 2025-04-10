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

            services.AddDbContext<GameDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PostgreSql"));
            }, ServiceLifetime.Scoped);


            services.AddScoped<GameDbContext>();
            services.AddScoped<GameDbContext>();
            return services;
        }
    }
}
