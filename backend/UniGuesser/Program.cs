
using PartyGame.Middleware;
using PartyGame.Extensions;
using PartyGame.DependencyInjection;
using PartyGame.Entities;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);

var app = builder.Build();
await SeedDatabase(app);
ConfigureMiddleware(app);
app.Run();

// Rejestracja usług
void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddApplicationDependencies(builder.Configuration);
}

// Seedowanie bazy danych
async Task SeedDatabase(WebApplication app)
{
    Console.WriteLine("##### Seeding database...");
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    db.Database.Migrate();
    Console.WriteLine("##### Db seeded...");

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
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Uniguesser API"));
     
    if (app.Environment.IsDevelopment())
    {
        // scalar docs
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("UniGuesser Api");
            options.Theme = ScalarTheme.DeepSpace;
            options.Layout = ScalarLayout.Classic;
            options.HideClientButton = true;
            options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
        });
    }


    app.UseAuthorization();
    app.MapControllers();
}
