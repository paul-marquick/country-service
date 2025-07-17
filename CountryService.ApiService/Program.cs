using Microsoft.EntityFrameworkCore;
using CountryService.DataAccess;
using CountryService.DataAccess.SqlServer;
using Scalar.AspNetCore;

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

        // https://stackoverflow.com/questions/79188513/addopenapi-adding-error-response-types-to-all-operations-net-9

        // Customise default API behaviour.
        builder.Services.AddEndpointsApiExplorer();

        // Add the Open API document generation services.
        builder.Services.AddOpenApi();

        // Data access.
        builder.Services.AddSingleton(
            new DatabaseOptions
            {
                ConnectionString = builder.Configuration.GetConnectionString("CountryServiceConnection")!
            });
        builder.Services.AddSingleton<ICountryDataAccess, CountryDataAccess>();

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ProblemDetailsExceptionFilter>();
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                return new ModelStateToValidationProblemDetails();
            };
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();
        app.UseStatusCodePages();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("TITLE_HERE")
             //       .WithDownloadButton(true)
                    .WithTheme(ScalarTheme.Purple)
                    .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
            });

            app.UseExceptionHandler();
        }

        // https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/service-defaults#provided-extension-methods

        // Maps the health checks endpoint to /health and the liveness endpoint to /alive.
        app.MapDefaultEndpoints();

        app.MapControllers();

        app.Run();
    }
}
