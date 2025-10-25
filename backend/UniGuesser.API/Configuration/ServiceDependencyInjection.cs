using Microsoft.Extensions.DependencyInjection;
using UniGuesser.Application.Services;
using UniGuesser.Application.Services.GameStartStrategies;
using UniGuesser.Application.Services.SaveFileService;
using UniGuesser.Application.UseCases.Accounts.AccountDetails;
namespace UniGuesser.API.Configuration
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ISaveFileService, SaveFileService>();
            services.AddScoped<IHttpContextAccessorService, HttpContextAccessorService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IGameRoundsGenerator, GameRoundsGenerator>();
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
