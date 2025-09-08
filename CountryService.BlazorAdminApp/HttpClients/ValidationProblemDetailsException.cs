using System;
using CountryService.BlazorAdminApp.Problems;

namespace CountryService.BlazorAdminApp.HttpClients;

/// <summary>
/// Used to throw a problem received from the web service.
/// </summary>
public class ValidationProblemDetailsException : Exception
{
    public ValidationProblemDetailsException(ExtendedValidationProblemDetails validationProblemDetails, Exception? innerException = null) :
        base(validationProblemDetails.Title, innerException)
    {
        ValidationProblemDetails = validationProblemDetails;
    }

    public ExtendedValidationProblemDetails ValidationProblemDetails { get; set; }
}
