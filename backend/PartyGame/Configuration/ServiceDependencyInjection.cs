using PartyGame.Services;
using PartyGame.Services.GameServices;
using PartyGame.Services.GameServices.GameStartStrategies;

namespace PartyGame.DependencyInjection
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {

            services.AddScoped<IGameSessionService, GameSessionService>();
            services.AddScoped<IPlaceService, PlaceService>();
            services.AddScoped<IPlaceQueueService, PlaceQueueService>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IHttpContextAccessorService, HttpContextAccessorService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRoundService, RoundService>();
            services.AddScoped<IGameGeolocationService, GameGeolocationService>();

            services.AddScoped<IGameRoundsGenerator, GameRoundsGenerator>();

            services.AddScoped<IGameStarter, GameStarter>();
            services.AddScoped<IStartGameStrategy, StartGameLogged>();
            services.AddScoped<StartGameLogged>(); // albo bezpośrednio
            services.AddScoped<StartGameUnlogged>();       // oba muszą być zarejestrowane


            return services;
        }
    }
}
