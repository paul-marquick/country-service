using CountryService.DataAccess;
using CountryService.Shared;
using CountryService.WebApi.Configuration;
using CountryService.WebApi.Middleware;
using CountryService.WebApi.Problems;
using OpenTelemetry.Metrics;
using Scalar.AspNetCore;
using Serilog;

namespace CountryService.WebApi;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Country Service Web Api start up.");

        const string allowAdminApp = "allowAdminApp";

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((ctx, lc) => lc
            .WriteTo.Console(outputTemplate: "level={Level:w} {Properties} msg={Message:lj} {NewLine}{Exception}")
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application_name", "CountryService.ApiService")
            .ReadFrom.Configuration(builder.Configuration)
        );

        // The options pattern uses classes to provide strongly typed access to groups of related settings.
        builder.Services.AddOptions();
        builder.AddAppSettings();
        Config config = builder.GetConfig();

        // Basic health probe. (See below for endpoint)
        builder.Services.AddHealthChecks();

        // Prometheus metrics.
        builder.Services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder.AddPrometheusExporter();
                builder.AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel");
            });

        // Access HttpContext through the injected IHttpContextAccessor.
        builder.Services.AddHttpContextAccessor();

        // Example: how to add header propagation to a named HTTP client.
        // Uncomment use header propagation below to use this. (approx line 160
        //builder.Services.AddHttpClient("planetsClient", c =>
        //{
        //    c.BaseAddress = new Uri("https://localhost:xxxx/");
        //}).AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));

        // Problem details service.
        builder.Services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = ctx =>
            {
                // You can add custom properties to the ProblemDetails instance.
                //  ctx.ProblemDetails.Extensions.Add("requestId", ctx.HttpContext.TraceIdentifier);
            });
        builder.Services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();

        // Generate Open API documentation for the API.
        builder.Services.AddEndpointsApiExplorer();

        // Add the Open API document generation services.
        builder.Services.AddOpenApi();

        // Code.
        builder.Services.AddSingleton<ProblemDetailsCreator>();

        // Data access.
        // Can simply switch between different database systems by editing the appsettings.json file.
        // Need to change the database system value and the database connection string.
        switch (config.DatabaseSystem)
        {
            case DatabaseSystem.SqlServer:
                Console.WriteLine("Using SQL Server database system.");
                builder.Services.AddSingleton<IDbConnectionFactory>(new DataAccess.SqlServer.DbConnectionFactory(builder.Configuration.GetConnectionString(Constants.CountryServiceConnectionStringName)!));
                builder.Services.AddSingleton<ICountryDataAccess, DataAccess.SqlServer.CountryDataAccess>();
                break;

            case DatabaseSystem.PostgreSql:
                Console.WriteLine("Using PostgreSQL database system.");
                builder.Services.AddSingleton<IDbConnectionFactory>(new DataAccess.PostgreSql.DbConnectionFactory(builder.Configuration.GetConnectionString(Constants.CountryServiceConnectionStringName)!));
                builder.Services.AddSingleton<ICountryDataAccess, DataAccess.PostgreSql.CountryDataAccess>();
                break;

            case DatabaseSystem.MySql:
                Console.WriteLine("Using MySQL database system.");
                builder.Services.AddSingleton<IDbConnectionFactory>(new DataAccess.MySql.DbConnectionFactory(builder.Configuration.GetConnectionString(Constants.CountryServiceConnectionStringName)!));
                builder.Services.AddSingleton<ICountryDataAccess, DataAccess.MySql.CountryDataAccess>();
                break;

            default:
                throw new ArgumentOutOfRangeException($"Start up exception: Unknown database system, specified in the appsettings, '{config.DatabaseSystem}'.");
        }        

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: allowAdminApp,
                policy  =>
                {
                    policy.AllowAnyOrigin() // WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });

        builder.Services.AddControllers(options =>
        {
            // Example: how to add a filter to the request processing pipeline.
            // options.Filters.Add<ProblemDetailsExceptionFilter>();
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                return new ModelStateToValidationProblemDetails(context.HttpContext.RequestServices.GetRequiredService<ILogger<ModelStateToValidationProblemDetails>>());
            };
        });

        WebApplication app = builder.Build();

        // Example: how to add middleware into the request processing pipeline.       
        // app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        // Custom middleware added using an IApplicationBuilder extension.
        app.UseCorrelationId();

        // Map health check endpoint.
        app.MapHealthChecks("/healthz");

        // Adds a StatusCodePages middleware with a default response handler that checks for
        // responses with status codes between 400 and 599 that do not have a body.
        app.UseStatusCodePages();

        // Adds a middleware to the pipeline that will catch exceptions, log them, and re-execute
        // the request in an alternate pipeline. The request will not be re-executed if the response has already started.
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            // OpenAPI document generation.
            app.MapOpenApi();

            // https://scalar.com/
            // Endpoints
            // /openapi/v1.json
            // /scalar
            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("Country Service")
                    .WithTheme(ScalarTheme.Purple)
                    .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
            });
        }

        // Adds a middleware that collect headers to be propagated to a HttpClient
        // E.g. correlation id.
        // app.UseHeaderPropagation();

        // Cross-Origin Resource Sharing.
        // Use the CORS policy, defined above. 
        app.UseCors(allowAdminApp);

        // Routing middleware to match the URLs of incoming requests and map them to actions.
        app.MapControllers();

        // Prometheus for metrics collection.
        app.MapPrometheusScrapingEndpoint();

        app.Run();
    }
}
