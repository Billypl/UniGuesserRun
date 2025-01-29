using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PartyGame;

public static class AuthenticationExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "BearerGame"; 
                options.DefaultScheme = "BearerGame";  
                options.DefaultChallengeScheme = "BearerGame";  
            })
            .AddJwtBearer("BearerGame", cfg =>  
            {
                var gameSettings = services.BuildServiceProvider().GetRequiredService<IOptions<AuthenticationGameSettings>>().Value;
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = gameSettings.JwtIssuer,
                    ValidAudience = gameSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(gameSettings.JwtKey))
                };
            })
            .AddJwtBearer("BearerAccount", cfg =>  
            {
                var accountSettings = services.BuildServiceProvider().GetRequiredService<IOptions<AuthenticationAccountSettings>>().Value;
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = accountSettings.JwtIssuer,
                    ValidAudience = accountSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accountSettings.JwtKey))
                };
            });
    }
}