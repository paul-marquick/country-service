using System.Net.Http;
using CountryService.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CountryService.WebApi.Controllers;

[Route(Paths.WebApi.Log.BasePath)]
[ApiController]
public class LogController(ILogger<ServiceInfoController> logger) : ControllerBase
{
    [HttpOptions]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public void Options()
    {
        Response.Headers.Allow = $"{HttpMethod.Options}, {HttpMethod.Post}";
        Response.ContentLength = 0;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult PostLogEntry([FromBody] Dtos.Log.Log log)
    {
        logger.LogDebug($"PostLogEntryAsync");

        logger.LogError(log.Message);

        return Ok();
    }
}
