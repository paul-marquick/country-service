using BlazorBootstrap;
using CountryService.BlazorAdminApp.HttpClients;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;

namespace CountryService.BlazorAdminApp.Problems;

public class ProblemDetailsExceptionHandler
{
    public static void HandleValidationProblemDetailsExceptionForForm(
        ValidationProblemDetailsException validationProblemDetailsException,
        ValidationMessageStore validationMessageStore,
        ToastService toastService,
        ToastType toastType = ToastType.Danger)
    {
        if (validationProblemDetailsException.ValidationProblemDetails.Type == Shared.Problems.ProblemType.FailedValidation &&
            validationProblemDetailsException.ValidationProblemDetails.Errors != null &&
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
        else
        {
            toastService.Notify(new(toastType, validationProblemDetailsException.ValidationProblemDetails.Title));
        }
    }

    public static void HandleValidationProblemDetailsExceptionToastOnly(
        ValidationProblemDetailsException validationProblemDetailsException,
        ToastService toastService,
        ToastType toastType = ToastType.Danger)
    {
        toastService.Notify(new(toastType, validationProblemDetailsException.ValidationProblemDetails.Title));
    }

    public static void HandleValidationProblemDetailsExceptionForAlert(
        ValidationProblemDetailsException validationProblemDetailsException,
        string? content,
        ToastService toastService,
        ToastType toastType = ToastType.Danger)
    {
        if (validationProblemDetailsException.ValidationProblemDetails.Type == Shared.Problems.ProblemType.FailedValidation &&
            validationProblemDetailsException.ValidationProblemDetails.Errors != null &&
            validationProblemDetailsException.ValidationProblemDetails.Errors.Count > 0)
        {
            foreach (KeyValuePair<string, string[]> error in validationProblemDetailsException.ValidationProblemDetails.Errors)
            {
                foreach (string message in error.Value)
                {
                    content += $"error, key: {error.Key}, message: {message}";
                }
            }
        }
        else
        {
            toastService.Notify(
                new(
                    toastType, 
                    validationProblemDetailsException.ValidationProblemDetails.Title +
                    validationProblemDetailsException.ValidationProblemDetails.Detail));
        }
    }



    public static string HandleValidationProblemDetailsExceptionTEMP(ValidationProblemDetailsException validationProblemDetailsException)
    {
        string content = $"Problem Details<br/><br/>" + 
            $"Status: {validationProblemDetailsException.ValidationProblemDetails.Status}<br/>" +
            $"Type: {validationProblemDetailsException.ValidationProblemDetails.Type}<br/>" +
            $"Title: {validationProblemDetailsException.ValidationProblemDetails.Title}<br/>" +
            $"Detail: {validationProblemDetailsException.ValidationProblemDetails.Detail}<br/>" +
            $"Instance: {validationProblemDetailsException.ValidationProblemDetails.Instance}<br/>" +
            $"RequestId: {validationProblemDetailsException.ValidationProblemDetails.RequestId}<br/>" +
            $"CorrelationId: {validationProblemDetailsException.ValidationProblemDetails.CorrelationId}<br/>";

        if (validationProblemDetailsException.ValidationProblemDetails.Errors == null ||
            validationProblemDetailsException.ValidationProblemDetails.Errors.Count == 0)
        {
            content += "<br/>No errors<br/>";
        }
        else
        { 
            foreach (KeyValuePair<string, string[]> error in validationProblemDetailsException.ValidationProblemDetails.Errors)
            {
                foreach (string message in error.Value)
                {
                    content += $"error, key: {error.Key}, message: {message}";
                }
            }
        }

        return content;
    }
}
