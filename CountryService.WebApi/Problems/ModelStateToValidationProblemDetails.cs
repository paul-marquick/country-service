using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace CountryService.WebApi.Problems;

/// <summary>
/// Converts ModelState validation failure to problem details instance containing validation errors.
/// </summary>
public class ModelStateToValidationProblemDetails(
    ILogger<ModelStateToValidationProblemDetails> logger,
    ProblemDetailsCreator problemDetailsCreator) : ActionResult
{
    public override Task ExecuteResultAsync(ActionContext context)
    {
        logger.LogDebug("ModelStateToValidationProblemDetails.ExecuteResultAsync");

        ValidationProblemDetails validationProblemDetails = problemDetailsCreator.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

        context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.HttpContext.Response.ContentType = Application.ProblemJson;

        //TODO: Remove, when tested that global is working. 

        // JsonSerializerOptions jsonSerializerOptions = new()
        // {
        //     PropertyNameCaseInsensitive = true,
        //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //     DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        // };

        // string validationProblemDetailsJsonString = JsonSerializer.Serialize(validationProblemDetails, jsonSerializerOptions);

        string validationProblemDetailsJsonString = JsonSerializer.Serialize(validationProblemDetails);

        byte[] validationProblemDetailsJsonBytes = Encoding.UTF8.GetBytes(validationProblemDetailsJsonString);
        context.HttpContext.Response.Body.WriteAsync(validationProblemDetailsJsonBytes);

        return Task.CompletedTask;
    }
}
