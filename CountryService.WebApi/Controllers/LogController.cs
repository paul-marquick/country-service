using System.Net.Http;
using CountryService.Dtos.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
    public ActionResult PostLogEntry([FromBody] Log log)
    {
        logger.LogDebug($"PostLogEntryAsync");

        logger.LogError(log.Message);

        return Ok();
    }
}
