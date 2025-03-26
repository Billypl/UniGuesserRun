using Microsoft.AspNetCore.DataProtection.Repositories;
using PartyGame.Entities;
using PartyGame.Repositories;

namespace PartyGame.DependencyInjection
{
    public static class RepositoryDependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPlacesRepository, PlacesRepository>();
            services.AddKeyedScoped<IScoreboardRepository, GameResultRepository>("GameResults", (provider, key) =>
            {
                var dbContext = provider.GetRequiredService<GameDbContext>();
                return new GameResultRepository(dbContext, "GameResults");
            });

            services.AddKeyedScoped<IScoreboardRepository, GameResultRepository>("GameHistory", (provider, key) =>
            {
                var dbContext = provider.GetRequiredService<GameDbContext>();
                return new GameResultRepository(dbContext, "GameHistory");
            });



            services.AddScoped<IGameSessionRepository, GameSessionRepository>();
            services.AddScoped<IPlacesRepository, PlacesRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IPlacesToCheckRepository, PlacesToCheckRepository>();

            return services;
        }
    }
}
