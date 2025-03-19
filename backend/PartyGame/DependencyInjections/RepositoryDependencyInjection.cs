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
            services.AddKeyedScoped<IScoreboardRepository, ScoreboardRepository>("GameResults", (provider, key) =>
            {
                var dbContext = provider.GetRequiredService<GameDbContext>();
                return new ScoreboardRepository(dbContext, "GameResults");
            });

            services.AddKeyedScoped<IScoreboardRepository, ScoreboardRepository>("GameHistory", (provider, key) =>
            {
                var dbContext = provider.GetRequiredService<GameDbContext>();
                return new ScoreboardRepository(dbContext, "GameHistory");
            });



            services.AddScoped<IGameSessionRepository, GameSessionRepository>();
            services.AddScoped<IPlacesRepository, PlacesRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IPlacesToCheckRepository, PlacesToCheckRepository>();

            return services;
        }
    }
}
