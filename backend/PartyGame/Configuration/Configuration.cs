using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PartyGame.Authorization;
using PartyGame.DependencyInjections;
using PartyGame.Entities;
using PartyGame.Extensions;
using PartyGame.Middleware;
using PartyGame.Settings;
using System.Text.Json.Serialization;

namespace PartyGame.DependencyInjection
{
    public static class Configuration
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDatabaseConfig(configuration)
                .AddCorsConfiguration(configuration)
                .AddAuthenticationConfig(configuration)
                .AddRepositories()
                .AddServices()
                .AddValidatorsConfig()
                .AddAutoMapperConfig();

            services.AddControllers();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
  
            services.Configure<GameSettings>(
                configuration.GetSection("GameSettings"));

            services.AddScoped<IAuthorizationHandler, HasGameSessionInDatabaseHandler>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasGameSessionInDatabase", policy =>
                    policy.Requirements.Add(new HasGameSessionInDatabase()));
            });

            services.AddScoped<GameSessionUpdater>();
            services.AddHostedService<GameSessionBackgroundUpdater>();

            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<Seeder>();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }

}
