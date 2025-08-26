using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CountryService.WebApi.Problems;

public class ProblemDetailsCreator(ILogger<ProblemDetailsCreator> logger) : IProblemDetailsCreator
{
    public ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int status,
        string type,
        string title,
        string detail)
    {
        string problemDetailsInstance = GenerateInstanceValue();

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
                { "correlationId", httpContext.Request.Headers["x-correlation-id"].FirstOrDefault() }
            }
        };
    }

    public ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
    {
        string problemDetailsInstance = GenerateInstanceValue();

        logger.LogDebug("ProblemDetailsCreator.CreateValidationProblemDetails, Problem details instance: {problemDetailsInstance}", problemDetailsInstance);

        return new(modelState)
        {
            Type = ProblemType.FailedValidation,
            Title = ProblemTitle.FailedValidation,
            Detail = "Invalid input.",
            Status = StatusCodes.Status400BadRequest,
            Instance = problemDetailsInstance,
            Extensions =
            {
                { "requestId", httpContext.TraceIdentifier },
                { "correlationId", httpContext.Request.Headers["x-correlation-id"].FirstOrDefault()}
            }
        };
    }

    private static string GenerateInstanceValue()
    {
        return Guid.NewGuid().ToString();
    }
}
