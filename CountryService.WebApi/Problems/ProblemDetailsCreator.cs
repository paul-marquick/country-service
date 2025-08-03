using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace CountryService.WebApi.Problems;

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
