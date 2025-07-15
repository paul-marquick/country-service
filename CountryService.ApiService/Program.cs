using Microsoft.EntityFrameworkCore;
using CountryService.DataAccess;
using CountryService.DataAccess.SqlServer;

namespace CountryService.ApiService;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire client integrations.
        builder.AddServiceDefaults();

        // https://learn.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-9.0#problem-details-service
        builder.Services.AddProblemDetails();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // Data access.
        builder.Services.AddSingleton(
            new DatabaseOptions
            {
                ConnectionString = builder.Configuration.GetConnectionString("CountryServiceConnection")!
            });
        builder.Services.AddSingleton<ICountryDataAccess, CountryDataAccess>();

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();
        app.UseStatusCodePages();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.MapOpenApi();
        }

        // https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/service-defaults#provided-extension-methods

        // Maps the health checks endpoint to /health and the liveness endpoint to /alive.
        app.MapDefaultEndpoints();

        app.MapControllers();

        app.Run();
    }
}
