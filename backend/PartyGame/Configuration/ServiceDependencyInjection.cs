﻿using PartyGame.Services;

namespace PartyGame.DependencyInjection
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {

            services.AddScoped<IGameSessionService, GameSessionService>();
            services.AddScoped<IPlaceService, PlaceService>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IHttpContextAccessorService, HttpContextAccessorService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRoundService, RoundService>();
            return services;
        }
    }
}
