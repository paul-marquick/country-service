using System;
using Microsoft.AspNetCore.Mvc;

namespace CountryService.BlazorAdminApp.HttpClients;

/// <summary>
/// Used to throw a problem received from the web service.
/// </summary>
public class ValidationProblemDetailsException : Exception
{
    public ValidationProblemDetailsException(ValidationProblemDetails validationProblemDetails, Exception? innerException = null) :
        base(validationProblemDetails.Title, innerException)
    {
        ValidationProblemDetails = validationProblemDetails;
    }

    public ValidationProblemDetails ValidationProblemDetails { get; set; }
}
