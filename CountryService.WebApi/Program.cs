using CountryService.DataAccess;
using CountryService.Shared;
using CountryService.WebApi.Configuration;
using CountryService.WebApi.Middleware;
using CountryService.WebApi.Problems;
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

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-9.0
        builder.Services.AddOptions();
        builder.AddAppSettings();
        Config config = builder.GetConfig();

        // https://www.code4it.dev/blog/serilog-correlation-id/
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));

        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.headerpropagationservicecollectionextensions.addheaderpropagation?view=aspnetcore-9.0
        // Example: how to add header propagation to a named HTTP client.
        //builder.Services.AddHttpClient("planetsClient", c =>
        //{
        //    c.BaseAddress = new Uri("https://localhost:xxxx/");
        //}).AddHeaderPropagation();

        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.problemdetailsservicecollectionextensions.addproblemdetails?view=aspnetcore-9.0
        builder.Services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = ctx =>
            {
                // You can add custom properties to the ProblemDetails instance.
                //  ctx.ProblemDetails.Extensions.Add("requestId", ctx.HttpContext.TraceIdentifier);
            });
        builder.Services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();

        // https://stackoverflow.com/questions/79188513/addopenapi-adding-error-response-types-to-all-operations-net-9

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
                builder.Services.AddSingleton<IDbConnectionFactory>(new DataAccess.SqlServer.DbConnectionFactory(builder.Configuration.GetConnectionString("CountryServiceConnection")!));
                builder.Services.AddSingleton<ICountryDataAccess, DataAccess.SqlServer.CountryDataAccess>();
                break;

            case DatabaseSystem.PostgreSql:
                Console.WriteLine("Using PostgreSQL database system.");
                builder.Services.AddSingleton<IDbConnectionFactory>(new DataAccess.PostgreSql.DbConnectionFactory(builder.Configuration.GetConnectionString("CountryServiceConnection")!));
                builder.Services.AddSingleton<ICountryDataAccess, DataAccess.PostgreSql.CountryDataAccess>();
                break;

            case DatabaseSystem.MySql:
                Console.WriteLine("Using MySQL database system.");
                builder.Services.AddSingleton<IDbConnectionFactory>(new DataAccess.MySql.DbConnectionFactory(builder.Configuration.GetConnectionString("CountryServiceConnection")!));
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
            // https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-9.0
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

        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.statuscodepagesextensions.usestatuscodepages?view=aspnetcore-9.0
        app.UseStatusCodePages();

        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.exceptionhandlerextensions.useexceptionhandler?view=aspnetcore-9.0
        app.UseExceptionHandler();

        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.hosting.hostingenvironmentextensions.isdevelopment?view=aspnetcore-9.0
        if (app.Environment.IsDevelopment())
        {
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-9.0
            app.MapOpenApi();

            // https://scalar.com/
            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("TITLE_HERE")
                    .WithTheme(ScalarTheme.Purple)
                    .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
            });
        }

        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.headerpropagationapplicationbuilderextensions.useheaderpropagation?view=aspnetcore-9.0
        // Used for the correlation id header.
        app.UseHeaderPropagation();

        // https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-9.0
        // Use the CORS policy, defined above. 
        app.UseCors(allowAdminApp);

        // https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-9.0
        app.MapControllers();

        app.Run();
    }
}
