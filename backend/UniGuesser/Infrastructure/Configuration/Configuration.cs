using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using UniGuesser.Domain.Middleware;
using UniGuesser.Domain.Entities;
using UniGuesser.Infrastructure.Extensions;
using UniGuesser.Infrastructure.Authorization;

namespace UniGuesser.Infrastructure.Configuration
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
           // services.AddOpenApi();
            services.AddSwaggerGen();

          


            return services;
        }
    }

}
