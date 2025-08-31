using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using CountryService.DataAccess;
using CountryService.DataAccess.ListQuery;
using CountryService.DataAccess.SqlServer.ListQuery;
using CountryService.Mappers.Country;
using CountryService.Shared;
using CountryService.WebApi.Configuration;
using CountryService.WebApi.ListQuery;
using CountryService.WebApi.Middleware;
using CountryService.WebApi.Patching;
using CountryService.WebApi.Problems;
using CountryService.WebApi.Startup;
using CountryService.WebApi.Startup.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.Extensions.Logging.AzureAppServices;

namespace CountryService.WebApi;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Country Service Web Api start up.");

    //    const string allowAdminApp = "allowAdminApp";

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        //    builder.Host.UseSerilog((ctx, lc) => lc
        //        .WriteTo.Console(outputTemplate: "level={Level:w} {Properties} msg={Message:lj} {NewLine}{Exception}")
        //        .Enrich.FromLogContext()
        //        .Enrich.WithProperty("Application_name", "CountryService.ApiService")
        //        .ReadFrom.Configuration(builder.Configuration)
        //    );

        builder.Logging.AddAzureWebAppDiagnostics();
        builder.Services.Configure<AzureFileLoggerOptions>(options =>
        {
            options.FileName = "logs-"; // Log file name prefix
            options.FileSizeLimit = 50 * 1024 * 1024; // 50 MB
            options.RetainedFileCountLimit = 5; // Keep last 5 log files
        });

        try
        {
            string dbConn = GetAzureKeyVaultValue(builder, "db-conn");

            Console.WriteLine($"dbConn from Key Vault: {dbConn}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception getting db-conn from Key Vault: {ex.Message}");
        }

            //    // The options pattern uses classes to provide strongly typed access to groups of related settings.
            //    builder.Services.AddOptions();
            //    builder.AddConfig();
            //    Config config = builder.GetConfig();

        //    string connectionString = default!;

        //    if (IsAzure())
        //    {
        //        Console.WriteLine("Running in Azure.");
        //        // Get connection string from Azure key vault.
        //        connectionString = GetAzureKeyVaultValue(builder, "db-conn");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Running locally.");
        //        // Get connection string from appsettings.json.
        //        connectionString = builder.Configuration["ConnectionString"]!;
        //    }

        //    // Add data access.
        //    //    builder.AddDataAccess(config.DatabaseSystem, builder.Configuration.GetConnectionString(Constants.CountryServiceConnectionStringName)!);
        //    builder.AddDataAccess(config.DatabaseSystem, connectionString);

        //    // Basic health probe. (See below for endpoint)
            builder.Services.AddHealthChecks();

    //    // Prometheus metrics.
    //    builder.Services.AddOpenTelemetry()
    //        .WithMetrics(builder =>
    //        {
    //            builder.AddPrometheusExporter();
    //            builder.AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel");
    //        });

    //    // Access HttpContext through the injected IHttpContextAccessor.
    //    builder.Services.AddHttpContextAccessor();

    //    // Example: how to add header propagation to a named HTTP client.
    //    // Uncomment use header propagation below to use this. (approx line 160
    //    //builder.Services.AddHttpClient("planetsClient", c =>
    //    //{
    //    //    c.BaseAddress = new Uri("https://localhost:xxxx/");
    //    //}).AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));

    //    // Problem details service.
    //    builder.Services.AddProblemDetails(options =>
    //        options.CustomizeProblemDetails = ctx =>
    //        {
    //            // You can add custom properties to the ProblemDetails instance.
    //            //  ctx.ProblemDetails.Extensions.Add("requestId", ctx.HttpContext.TraceIdentifier);
    //        });
    //    builder.Services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();

    //    // Generate Open API documentation for the API.
    //    builder.Services.AddEndpointsApiExplorer();

    //    // Add the Open API document generation services.
    //    builder.Services.AddOpenApi();

    //    // Add custom code.
    //    builder.AddCode();

    //    builder.Services.AddCors(options =>
    //    {
    //        options.AddPolicy(name: allowAdminApp,
    //            policy =>
    //            {
    //                policy.AllowAnyOrigin() // WithOrigins("http://localhost:4200") // Implement ICorsPolicyProvider.
    //                    .AllowAnyMethod()
    //                    .AllowAnyHeader();
    //            });
    //    });

    //    builder.Services.AddControllers(options =>
    //    {
    //        // https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-9.0
    //        options.InputFormatters.Insert(0, JsonPatchInputFormatter.GetJsonPatchInputFormatter());

    //        // Example: how to add a filter to the request processing pipeline.
    //        // options.Filters.Add<ProblemDetailsExceptionFilter>();
    //    })
    //    .AddJsonOptions(options =>
    //    {
    //        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    //        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;           
    //        options.JsonSerializerOptions.PropertyNamingPolicy=JsonNamingPolicy.CamelCase;
    //    })
    //    .ConfigureApiBehaviorOptions(options =>
    //    {
    //        options.InvalidModelStateResponseFactory = context =>
    //        {
    //            return new ModelStateToValidationProblemDetails(
    //                context.HttpContext.RequestServices.GetRequiredService<ILogger<ModelStateToValidationProblemDetails>>(), 
    //                context.HttpContext.RequestServices.GetRequiredService<IProblemDetailsCreator>());
    //        };
    //    }).AddNewtonsoftJson();

        WebApplication app = builder.Build();

        app.Logger.LogInformation("WebApplication Build().");


    //    // Example: how to add middleware into the request processing pipeline.       
    //    // app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    //    // Custom middleware added using an IApplicationBuilder extension.
    //    app.UseCorrelationId();

    //    // Map health check endpoint.
        app.MapHealthChecks("/healthz");

    //    // Adds a StatusCodePages middleware with a default response handler that checks for
    //    // responses with status codes between 400 and 599 that do not have a body.
    //    app.UseStatusCodePages();

    //    // Adds a middleware to the pipeline that will catch exceptions, log them, and re-execute
    //    // the request in an alternate pipeline. The request will not be re-executed if the response has already started.
    //    app.UseExceptionHandler();

    //    // WARNING: Using OpenAPI and Scalar in ALL environments.
    //    //    if (app.Environment.IsDevelopment())
    //    //    {

    //        // OpenAPI document generation.
    //        app.MapOpenApi();

    //        // https://scalar.com/
    //        // Endpoints
    //        // /openapi/v1.json
    //        // /scalar
    //        app.MapScalarApiReference(options =>
    //        {
    //            options
    //                .WithTitle("Country Service")
    //                .WithTheme(ScalarTheme.Purple)
    //                .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
    //        });
    ////    }

    //    // Adds a middleware that collect headers to be propagated to a HttpClient
    //    // E.g. correlation id.
    //    // app.UseHeaderPropagation();

    //    // Cross-Origin Resource Sharing.
    //    // Use the CORS policy, defined above. 
    //    app.UseCors(allowAdminApp);

    //    // Routing middleware to match the URLs of incoming requests and map them to actions.
    //    app.MapControllers();

    //    // Prometheus for metrics collection.
    //    app.MapPrometheusScrapingEndpoint();

        app.Run();
    }

    private static string GetAzureKeyVaultValue(WebApplicationBuilder builder, string secretName)
    {
        const string keyVaultName = "pm-key-vault";

        Uri keyVaultEndpoint = new Uri($"https://{keyVaultName}.vault.azure.net/");
        SecretClient secretClient = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());

        KeyVaultSecret kvs = secretClient.GetSecret(secretName);
        return kvs.Value;
    }

    private static bool IsAzure()
    {
        return !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")) &&
            !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
    }
}
