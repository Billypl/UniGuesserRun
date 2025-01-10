using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using PartyGame;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PartyGame.Entities;
using PartyGame.Services;
using MongoDB.Driver;
using PartyGame.Middleware;
using PartyGame.Models;
using PartyGame.Repositories;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);

var app = builder.Build();
await SeedDatabase(app);
ConfigureMiddleware(app);
app.Run();

// Rejestracja us�ug
void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddLogging(logging =>
    {
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Debug);
    });

    // dodanie bazy
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddControllers();
    builder.Services.AddSingleton<IMongoClient>(sp =>
    {
         var connectionString = "mongodb://localhost:27017";
        //var connectionString = "mongodb://root:example@mongo:27017";
        return new MongoClient(connectionString);
    });
    builder.Services.AddScoped<GameDbContext>();

    // mapper
    builder.Services.AddAutoMapper(typeof(PlacesMappingProfile));
    builder.Services.AddAutoMapper(builder.GetType().Assembly);

    // authentication 
    var authenticationSettings = builder.Configuration.GetSection("Authentication").Get<AuthenticationSettings>();
    builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("Authentication"));
    builder.Services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = "Bearer";
        option.DefaultScheme = "Bearer";
        option.DefaultChallengeScheme = "Bearer";
    }).AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.SaveToken = true;
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = authenticationSettings.JwtIssuer,
            ValidAudience = authenticationSettings.JwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
        };
    });

    // Cors
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigins", policy =>
        {
            policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod();
        });
    });
    // Konwersja poziomu trudnosci JSON string -> enum 
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    // Serwisy
    builder.Services.AddScoped<ErrorHandlingMiddleware>();
    builder.Services.AddScoped<IScoreboardRepository, ScoreboardRepository>();
    builder.Services.AddScoped<IGameSessionRepository, GameSessionRepository>();
    builder.Services.AddScoped<IPlacesRepository, PlacesRepository>();
    builder.Services.AddScoped<IGameSessionService, GameSessionService>();
    builder.Services.AddScoped<IPlaceService, PlaceService>();
    builder.Services.AddScoped<IScoreboardService, ScoreboardService>();
    builder.Services.AddScoped<IGameService, GameService>();
    builder.Services.AddScoped<IHttpContextAccessorService, HttpContextAccessorService>();

    builder.Services.AddScoped<Seeder>();
    builder.Services.AddSwaggerGen();

    // Walidatory
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<StartDataValidator>();
}

// Seedowanie bazy danych
async Task SeedDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
    await seeder.Seed();
}


// Konfiguracja middleware
void ConfigureMiddleware(WebApplication app)
{
    app.UseCors("AllowSpecificOrigins");
    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseAuthentication();
    app.UseHttpsRedirection();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PartyGame API"));
    app.UseAuthorization();
    app.MapControllers();
}
