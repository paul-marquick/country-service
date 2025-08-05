using CountryService.Shared;
using CountryService.WebApi.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


// Want 

// health endpoint
// prometheus endpoint
// with query param for db check
// system info endpoint
// system config endpoint

namespace CountryService.WebApi.Controllers;

[Route("system-info")]
[ApiController]
public class ServiceInfoController(
    ILogger<ServiceInfoController> logger,
    IConfiguration configuration,
    IOptionsMonitor<Config> optionsMonitorConfig) : ControllerBase
{
    [HttpGet]
    public ActionResult<Config> Get()
    {
        logger.LogDebug("Get");

     //   var connString = configuration.GetConnectionString("Constants.CountryServiceConnectionStringName");

        var sysInfo = new
        {
            DatabaseSystem = optionsMonitorConfig.CurrentValue.DatabaseSystem,
            ServiceName = "CountryService",
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            MachineName = Environment.MachineName,
            OSVersion = Environment.OSVersion.ToString(),
            FrameworkDescription = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
            RuntimeIdentifier = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier,
            ProcessId = Environment.ProcessId,
            Uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime()
        };

        return Ok(sysInfo);
    }
}
