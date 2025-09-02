using BlazorBootstrap;
using CountryService.BlazorAdminApp.HttpClients;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;

namespace CountryService.BlazorAdminApp.Problems;

public class ValidationProblemDetailsExceptionHandler
{
    public static void HandleValidationProblemDetailsException(
        ValidationProblemDetailsException validationProblemDetailsException,
        ValidationMessageStore validationMessageStore, 
        ToastService toastService, 
        ToastType toastType = ToastType.Danger)
    {
        if (validationProblemDetailsException.ValidationProblemDetails.Type == Shared.Problems.ProblemType.FailedValidation)
        {
            if (validationProblemDetailsException.ValidationProblemDetails.Errors != null &&
                validationProblemDetailsException.ValidationProblemDetails.Errors.Count > 0)
            {
                foreach (KeyValuePair<string, string[]> error in validationProblemDetailsException.ValidationProblemDetails.Errors)
                {
                    foreach (string message in error.Value)
                    {
                        validationMessageStore.Add(() => error.Key, message);
                    }
                }
            }
        }
        else
        {
            toastService.Notify(new(toastType, validationProblemDetailsException.ValidationProblemDetails.Title));
        }
    }
}
