namespace CountryService.WebApi.Problems;

/// <summary>
/// Global exception handler that translates exceptions into ProblemDetails responses and sends them via the ProblemDetailsService.
/// </summary>
/// <see cref="https://timdeschryver.dev/blog/translating-exceptions-into-problem-details-responses"/>
public class ExceptionToProblemDetailsHandler(
    ILogger<ExceptionToProblemDetailsHandler> logger,
    IProblemDetailsService problemDetailsService) : Microsoft.AspNetCore.Diagnostics.IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogDebug("TryHandleAsync");

        string problemDetailsInstance = Guid.NewGuid().ToString();

        logger.LogError(
            exception,
            "TryHandleAsync, error has occurred. Problem details instance: {problemDetailsInstance}", problemDetailsInstance);

        // Replace some context which has been lost due to the standard exception handler reissuing the response.
        httpContext.Response.Headers["x-correlation-id"] = httpContext.Request.Headers["x-correlation-id"].FirstOrDefault();

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails =
                {
                    Title = ProblemTitle.InternalServerError,
                    Detail = "An error has occurred. The Administrator has been notified.",
                    Type = ProblemType.InternalServerError,
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = problemDetailsInstance,
                    Extensions =
                    {
                        { "requestId", httpContext.TraceIdentifier },
                        { "correlationId", httpContext.Request.Headers["x-correlation-id"].FirstOrDefault()}
                    }
                },
            Exception = exception
        });
    }
}
