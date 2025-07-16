using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CountryService.ApiService;

public class ProblemDetailsException : Exception
{
    public ProblemDetailsException(HttpStatusCode httpStatusCode, string title, string detail)
    {
        StatusCode = (int)httpStatusCode;
        ProblemDetails = new ProblemDetails
        {
            Status = StatusCode,
            Type = GetProblemDetailsType(),
            Title = title,
            Detail = detail,
            Instance = Guid.NewGuid().ToString()
        };
    }

    public int StatusCode { get; }

    public ProblemDetails ProblemDetails { get; }

    private string GetProblemDetailsType()
    {
        switch (StatusCode)
        {
            // Client errors.

            case 400:
                return "https://datatracker.ietf.org/doc/html/rfc9110#name-400-bad-request";

            case 401:
                return "https://datatracker.ietf.org/doc/html/rfc9110#name-401-unauthorized";

            case 402:
                return "https://datatracker.ietf.org/doc/html/rfc9110#name-402-payment-required";

            case 403:
                return "https://datatracker.ietf.org/doc/html/rfc9110#name-403-forbidden";

            case 404:
                return "https://datatracker.ietf.org/doc/html/rfc9110#name-404-not-found";

            case 405:
                return "https://datatracker.ietf.org/doc/html/rfc9110#name-405-method-not-allowed";

            case 406:
                return "https://datatracker.ietf.org/doc/html/rfc9110#name-406-not-acceptable";

            // Server errors.

            case 500:
                return "https://datatracker.ietf.org/doc/html/rfc9110#name-500-internal-server-error";

            default:
                return "unknown";
        }
    }
}
