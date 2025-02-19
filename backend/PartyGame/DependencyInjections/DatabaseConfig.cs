using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.DependencyInjections
{
    public static class DatabaseConfig
    {
        public static IServiceCollection AddDatabaseConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IMongoClient>(sp =>
            {
                var connectionString = "mongodb://localhost:27017";
                //var connectionString = "mongodb://root:example@mongo:27017";
                return new MongoClient(connectionString);
            });


            services.AddScoped<GameDbContext>();
            services.AddScoped<GameDbContext>();
            return services;
        }
    }
}
