using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace CountryService.WebApi.Problems;

/// <summary>
/// Converts ModelState validation failure to problem details instance containing validation errors.
/// </summary>
public class ModelStateToValidationProblemDetails(ILogger<ModelStateToValidationProblemDetails> logger) : ActionResult
{
    public override Task ExecuteResultAsync(ActionContext context)
    {
        string problemDetailsInstance = Guid.NewGuid().ToString();

        logger.LogDebug("ExecuteResultAsync, Problem details instance: {problemDetailsInstance}", problemDetailsInstance);

        ValidationProblemDetails validationProblemDetails = new(context.ModelState)
        {
            Type = ProblemType.FailedValidation,
            Title = ProblemTitle.FailedValidation,
            Detail = "Invalid input.",
            Status = StatusCodes.Status400BadRequest,
            Instance = problemDetailsInstance,
            Extensions =
            {
                { "requestId", context.HttpContext.TraceIdentifier },
                { "correlationId", context.HttpContext.Request.Headers["x-correlation-id"].FirstOrDefault()}
            }
        };

        context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.HttpContext.Response.ContentType = Application.ProblemJson;

        JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        string validationProblemDetailsJsonString = JsonSerializer.Serialize(validationProblemDetails, jsonSerializerOptions);

        byte[] validationProblemDetailsJsonBytes = Encoding.UTF8.GetBytes(validationProblemDetailsJsonString);

        context.HttpContext.Response.Body.WriteAsync(validationProblemDetailsJsonBytes);

        return Task.CompletedTask;
    }
}