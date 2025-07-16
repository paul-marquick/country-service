using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static System.Net.Mime.MediaTypeNames;

namespace CountryService.ApiService;

public class ProblemDetailsExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is ProblemDetailsException problemDetailsException)
        {
            context.Result = new JsonResult(problemDetailsException.ProblemDetails)
            {
                StatusCode = problemDetailsException.StatusCode,
                ContentType = Application.ProblemJson
            };

            context.ExceptionHandled = true;
        }
    }
}
