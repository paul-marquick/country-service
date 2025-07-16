using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace CountryService.ApiService;

/// <summary>
/// Converts ModelState validation failure to problem details instance containing validation errors.
/// </summary>
public class ModelStateToValidationProblemDetails : ActionResult
{
    public override Task ExecuteResultAsync(ActionContext context)
    {
        ValidationProblemDetails validationProblemDetails = new(context.ModelState)
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-400-bad-request",
            Title = "Failed validation",
            Detail = "Invalid input.",
            Status = StatusCodes.Status400BadRequest,
            Instance = Guid.NewGuid().ToString()
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