using Microsoft.AspNetCore.Mvc;

namespace CountryService.WebApi.Problems;

/// <summary>
/// Global exception handler that translates exceptions into ProblemDetails responses and sends them via the ProblemDetailsService.
/// </summary>
/// <see cref="https://timdeschryver.dev/blog/translating-exceptions-into-problem-details-responses"/>
public class ExceptionToProblemDetailsHandler(
    ILogger<ExceptionToProblemDetailsHandler> logger,
    IProblemDetailsService problemDetailsService, 
    ProblemDetailsCreator problemDetailsCreator) : Microsoft.AspNetCore.Diagnostics.IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogDebug("ExceptionToProblemDetailsHandler.TryHandleAsync");

        // Replace some context which has been lost due to the standard exception handler reissuing the response.
        httpContext.Response.Headers["x-correlation-id"] = httpContext.Request.Headers["x-correlation-id"].FirstOrDefault();
        
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        ProblemDetails problemDetails = problemDetailsCreator.CreateProblemDetails(
            httpContext,
            StatusCodes.Status500InternalServerError,
            ProblemType.InternalServerError,
            ProblemTitle.InternalServerError,
            "An error has occurred. The Administrator has been notified.");
        
        logger.LogError(
            exception,
            "ExceptionToProblemDetailsHandler.TryHandleAsync, error has occurred. Problem details instance: {problemDetailsInstance}",
            problemDetails.Instance);

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });            
    }
}
