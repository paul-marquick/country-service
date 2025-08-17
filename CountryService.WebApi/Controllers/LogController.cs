using CountryService.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CountryService.WebApi.Controllers;

[Route("log")]
[ApiController]
public class LogController(ILogger<ServiceInfoController> logger) : ControllerBase
{
    [HttpOptions]
    public void Options()
    {
        Response.Headers.Allow = $"{HttpMethod.Options}, {HttpMethod.Post}";
        Response.ContentLength = 0;
    }

    [HttpPost]
    public ActionResult PostLogEntry([FromBody] LogEntry logEntry)
    {
        logger.LogDebug($"PostLogEntryAsync");

        logger.LogError(logEntry.Message);

        return Ok();
    }
}
