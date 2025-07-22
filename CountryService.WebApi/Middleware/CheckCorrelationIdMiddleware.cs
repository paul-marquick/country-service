namespace CountryService.WebApi.Middleware;

public class CheckCorrelationIdMiddleware
{
    private readonly RequestDelegate next;

    public CheckCorrelationIdMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("x-correlation-id", out var correlationId))
        {
            Console.WriteLine("x-correlation-id not found in headers.");
            context.Request.Headers["x-correlation-id"] = Guid.NewGuid().ToString();
        }

        await next(context);
    }
}
