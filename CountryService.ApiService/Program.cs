using Microsoft.EntityFrameworkCore;
using CountryService.DataAccess;
using CountryService.DataAccess.SqlServer;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

namespace CountryService.ApiService;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((ctx, lc) => lc
            .WriteTo.Console(outputTemplate: "level={Level:w} {Properties} msg={Message:lj} {NewLine}{Exception}")
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application_name", "CountryService.ApiService")
            .Enrich.WithCorrelationIdHeader("x-correlation-id")
        );

        // Add service defaults & Aspire client integrations.
        builder.AddServiceDefaults();

        // https://www.code4it.dev/blog/serilog-correlation-id/
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));

        // Example just to show how to add header propagation to a named HTTP client.
        //builder.Services.AddHttpClient("cars_system", c =>
        //{
        //    c.BaseAddress = new Uri("https://localhost:xxxx/");
        //}).AddHeaderPropagation();

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

        // create proper middleware class.

        app.Use(async (context, next) =>
        {
            Console.WriteLine("Before next middleware");

            if (!context.Request.Headers.TryGetValue("x-correlation-id", out var correlationId))
            {
                Console.WriteLine("x-correlation-id not found in headers."); 
                context.Request.Headers.Add("x-correlation-id", Guid.NewGuid().ToString());
            }            

            await next(); // Calls the next middleware in the pipeline
            Console.WriteLine("After next middleware");
        });

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

        app.UseHeaderPropagation();

        // https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/service-defaults#provided-extension-methods

        // Maps the health checks endpoint to /health and the liveness endpoint to /alive.
        app.MapDefaultEndpoints();

        app.MapControllers();

        app.Run();
    }
}
