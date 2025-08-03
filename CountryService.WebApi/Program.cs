using Microsoft.EntityFrameworkCore;
using CountryService.DataAccess;
using CountryService.DataAccess.SqlServer;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using CountryService.WebApi.Middleware;
using CountryService.WebApi.Problems;

namespace CountryService.WebApi;

internal class Program
{
    private static void Main(string[] args)
    {
        const string allowAdminApp = "allowAdminApp";

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((ctx, lc) => lc
            .WriteTo.Console(outputTemplate: "level={Level:w} {Properties} msg={Message:lj} {NewLine}{Exception}")
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application_name", "CountryService.ApiService")
        //    .Enrich.WithCorrelationIdHeader("x-correlation-id")
            .ReadFrom.Configuration(builder.Configuration)
        );

        // https://www.code4it.dev/blog/serilog-correlation-id/
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));

        // Example just to show how to add header propagation to a named HTTP client.
        //builder.Services.AddHttpClient("cars_system", c =>
        //{
        //    c.BaseAddress = new Uri("https://localhost:xxxx/");
        //}).AddHeaderPropagation();

        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.problemdetailsservicecollectionextensions.addproblemdetails?view=aspnetcore-9.0
        builder.Services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = ctx =>
            {
                // Microsoft code NEVER works 100% in the real world. It's all workarounds and hacks.
                //  ctx.ProblemDetails.Extensions.Add("requestId", ctx.HttpContext.TraceIdentifier);
            });
        builder.Services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();

        // https://stackoverflow.com/questions/79188513/addopenapi-adding-error-response-types-to-all-operations-net-9

        // Customise default API behaviour.
        builder.Services.AddEndpointsApiExplorer();

        // Add the Open API document generation services.
        builder.Services.AddOpenApi();

        // Code.
        builder.Services.AddSingleton<ProblemDetailsCreator>();

        // Data access.
        builder.Services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(builder.Configuration.GetConnectionString("CountryServiceConnection")!));
        builder.Services.AddSingleton<ICountryDataAccess, CountryDataAccess>();

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
 // Example:         //  options.Filters.Add<ProblemDetailsExceptionFilter>();
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                return new ModelStateToValidationProblemDetails(context.HttpContext.RequestServices.GetRequiredService<ILogger<ModelStateToValidationProblemDetails>>());
            };
        });

        WebApplication app = builder.Build();

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-9.0
        app.UseMiddleware<CheckCorrelationIdMiddleware>();

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
