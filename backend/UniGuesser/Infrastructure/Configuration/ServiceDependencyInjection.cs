using UniGuesser.Application.UseCases.Accounts;
using UniGuesser.Application.UseCases.Accounts.AccountDetails;
using UniGuesser.Domain.Services;
using UniGuesser.Domain.Services.GameServices;
using UniGuesser.Domain.Services.GameServices.GameStartStrategies;

namespace UniGuesser.Infrastructure.Configuration
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {

            services.AddScoped<IGameSessionService, GameSessionService>();

            services.AddScoped<IHttpContextAccessorService, HttpContextAccessorService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRoundService, RoundService>();
            services.AddScoped<IGameGeolocationService, GameGeolocationService>();

            services.AddScoped<IGameRoundsGenerator, GameRoundsGenerator>();

            services.AddScoped<IGameStarter, GameStarter>();
            services.AddScoped<IStartGameStrategy, StartGameLogged>();
            services.AddScoped<StartGameLogged>(); // albo bezpośrednio
            services.AddScoped<StartGameUnlogged>();       // oba muszą być zarejestrowane


            services.AddScoped<AccountDetailsHandler>();

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(AccountDetailsHandler).Assembly));



            return services;
        }
    }
}
