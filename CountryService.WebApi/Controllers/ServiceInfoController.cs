using System;
using System.Net.Http;
using CountryService.WebApi.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static System.Net.Mime.MediaTypeNames;

namespace CountryService.WebApi.Controllers;

[Route("service-info")]
[ApiController]
public class ServiceInfoController(
    ILogger<ServiceInfoController> logger,
    IOptionsMonitor<Config> optionsMonitorConfig) : ControllerBase
{
    [HttpOptions]
    public void Options()
    {
        Response.Headers.Allow = $"{HttpMethod.Options}, {HttpMethod.Head}, {HttpMethod.Get}";
        Response.ContentLength = 0;
    }

    [HttpHead]
    [HttpGet]
    public ActionResult<Config> Get()
    {
        string method = HttpContext.Request.Method;

        logger.LogDebug($"Get, method: {method}");

        var sysInfo = new
        {
            DatabaseSystem = optionsMonitorConfig.CurrentValue.DatabaseSystem,
            ServiceName = "CountryService",
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            MachineName = Environment.MachineName,
            OSVersion = Environment.OSVersion.ToString(),
            IsDebugLogLevelEnabled = logger.IsEnabled(LogLevel.Debug),
            FrameworkDescription = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
            RuntimeIdentifier = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier,
            ProcessId = Environment.ProcessId,
            Uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime()
        };

        if (method == HttpMethod.Head.Method)
        {
            Response.Headers.ContentType = Application.Json;
            Response.Headers.ContentLength = sysInfo.ToString()!.Length;

            return new EmptyResult();
        }
        else
        {
            return Ok(sysInfo);
        }
    }
}
