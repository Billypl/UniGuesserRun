using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UniGuesser.API.Configuration
{
    public static class CorsConfig
    {
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", policy =>
                {
                    var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
                    policy.WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
