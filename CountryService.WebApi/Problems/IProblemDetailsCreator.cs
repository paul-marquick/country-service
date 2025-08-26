using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CountryService.WebApi.Problems;

/// <summary>
/// Provides functionality to create <see cref="ProblemDetails"/> instances for HTTP responses.
/// </summary>
/// <remarks>This class is designed to generate <see cref="ProblemDetails"/> objects that conform to the RFC 7807
/// specification for problem details in HTTP APIs. It includes additional metadata,  such as request identifiers and
/// correlation IDs, to aid in debugging and tracing.</remarks>
/// <param name="logger"></param>
public interface IProblemDetailsCreator
{
    ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int status,
        string type,
        string title,
        string detail);

    ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState);
}
