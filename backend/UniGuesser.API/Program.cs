using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using UniGuesser.API.Configuration;
using UniGuesser.API.Extensions;
using UniGuesser.API.Middleware;
using UniGuesser.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);

var app = builder.Build();


var filesFolder = app.Configuration["FileSaveData:SaveFolder"];

if (!string.IsNullOrEmpty(filesFolder))
{
    var webRootPath = builder.Environment.WebRootPath ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
    var filesFolderPath = Path.Combine(webRootPath, filesFolder);

    // Utwórz katalog jeśli nie istnieje
    Directory.CreateDirectory(filesFolderPath);

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(filesFolderPath),
        RequestPath = $"/{filesFolder}"
    });

    Console.WriteLine($"Path of the files: {filesFolderPath}");
}
else
{
    Console.WriteLine("WARNING: FileSaveData:SaveFolder not configured");
}

app.Services.MigrateDatabase();
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
    // db.Database.Migrate();
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
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "UniGuesser API V1"); });


    if (app.Environment.IsDevelopment())
    {
        // scalar docs
        //app.MapOpenApi();
        //app.MapScalarApiReference(options =>
        //{
        //    options.WithTitle("UniGuesser Api");
        //    options.Theme = ScalarTheme.DeepSpace;
        //    options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
        //});
    }


    app.UseAuthorization();
    app.MapControllers();
}