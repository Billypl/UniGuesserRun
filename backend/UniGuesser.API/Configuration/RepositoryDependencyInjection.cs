using Microsoft.Extensions.DependencyInjection;
using UniGuesser.Domain.Repositories;
using UniGuesser.Infrastructure;
using UniGuesser.Infrastructure.Repositories;

namespace UniGuesser.API.Configuration;

public static class RepositoryDependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPlacesRepository, PlacesRepository>();
        services.AddScoped<IGameSessionRepository, GameSessionRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IRoundRepository, RoundRepository>();

        return services;
    }
}