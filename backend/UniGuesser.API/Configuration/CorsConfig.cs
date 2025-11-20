using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UniGuesser.API.Configuration;

public static class CorsConfig
{
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", policy =>
            {
                var isDevelopment = configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Development";
                
                if (isDevelopment)
                {
                    // In development, allow all origins
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                }
                else
                {
                    // Production: strict origin checking
                    var hostIp = Environment.GetEnvironmentVariable("HOST_IP") ?? 
                                 configuration["ServerEnvironment:HostIp"] ?? 
                                 "localhost";
                    
                    var frontendPort = Environment.GetEnvironmentVariable("FRONTEND_PORT") ?? "3000";
                    
                    var allowedOrigins = new List<string>
                    {
                        "https://localhost:3000",
                        "https://127.0.0.1:3000",
                        $"https://localhost:{frontendPort}",
                        $"https://127.0.0.1:{frontendPort}"
                    };
                    
                    if (hostIp != "localhost" && !string.IsNullOrEmpty(hostIp))
                    {
                        allowedOrigins.Add($"https://{hostIp}:{frontendPort}");
                    }
                    
                    var configOrigins = configuration["CorsSettings:AllowedOrigins"];
                    if (!string.IsNullOrEmpty(configOrigins))
                    {
                        allowedOrigins.AddRange(configOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim()));
                    }
                    
                    policy.WithOrigins(allowedOrigins.Distinct().ToArray())
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                }
            });
        });

        return services;
    }
}