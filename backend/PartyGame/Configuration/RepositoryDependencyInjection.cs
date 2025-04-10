using Microsoft.AspNetCore.DataProtection.Repositories;
using PartyGame.Entities;
using PartyGame.Repositories;
using PartyGame.Services;

namespace PartyGame.DependencyInjection
{
    public static class RepositoryDependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPlacesRepository, PlacesRepository>();
            services.AddScoped<IGameSessionRepository, GameSessionRepository>();
            services.AddScoped<IPlacesRepository, PlacesRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRoundRepository, RoundRepository>();

            return services;
        }
    }
}
