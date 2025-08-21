using Serilog.Context;

namespace CountryService.WebApi.Middleware;

/// <summary>
/// A correlation ID is a unique identifier assigned to a request as it travels through a system, 
/// especially useful in distributed systems for tracing the request's path and troubleshooting issues.
/// </summary>
/// <see cref="https://microsoft.github.io/code-with-engineering-playbook/observability/correlation-id/"/>
public class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        // If we haven't received a correlation ID in the request headers, generate a new one.
        if (!httpContext.Request.Headers.TryGetValue("x-correlation-id", out var correlationId))
        {
            httpContext.Request.Headers["x-correlation-id"] = Guid.NewGuid().ToString();
        }

        // Log the correlation ID and add it to the response headers.
        LogContext.PushProperty("correlationId", httpContext.Request.Headers["x-correlation-id"]);
        httpContext.Response.Headers["x-correlation-id"] = httpContext.Request.Headers["x-correlation-id"].FirstOrDefault();

        await next(httpContext);
    }
}
