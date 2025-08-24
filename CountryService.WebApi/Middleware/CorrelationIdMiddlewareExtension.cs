using Microsoft.AspNetCore.Builder;

namespace CountryService.WebApi.Middleware;

/// <summary>
/// Extension method used to add the middleware to the HTTP request pipeline.
/// </summary>
public static class CorrelationIdMiddlewareExtension
{
    // E.g. app.UseCorrelationId();
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationIdMiddleware>();
    }
}
