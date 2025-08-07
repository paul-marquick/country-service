using Microsoft.AspNetCore.Mvc;

namespace CountryService.WebApi.Problems;

/// <summary>
/// Provides functionality to create <see cref="ProblemDetails"/> instances for HTTP responses.
/// </summary>
/// <remarks>This class is designed to generate <see cref="ProblemDetails"/> objects that conform to the RFC 7807
/// specification for problem details in HTTP APIs. It includes additional metadata,  such as request identifiers and
/// correlation IDs, to aid in debugging and tracing.</remarks>
/// <param name="logger"></param>
public class ProblemDetailsCreator(ILogger<ProblemDetailsCreator> logger)
{
    public ProblemDetails CreateProblemDetails(
        HttpContext httpContext, 
        int status,
        string type,
        string title,
        string detail)
    {
        string problemDetailsInstance = Guid.NewGuid().ToString();

        logger.LogDebug(
            "CreateProblemDetails, status: {Status}, type: {Type}, title: {Title}, detail: {Detail}, instance: {problemDetailsInstance}",
            status, 
            type, 
            title, 
            detail, 
            problemDetailsInstance);

        return new ProblemDetails
        {
            Type = type,
            Title = title,
            Detail = detail,
            Status = status,
            Instance = problemDetailsInstance,
            Extensions =
            {
                { "requestId", httpContext.TraceIdentifier },
                { "x-correlation-id", httpContext.Request.Headers["x-correlation-id"].FirstOrDefault() }
            }
        };
    }
}
