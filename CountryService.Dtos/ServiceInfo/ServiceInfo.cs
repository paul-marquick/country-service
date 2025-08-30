using System;

namespace CountryService.Dtos.ServiceInfo;

public record ServiceInfo
{
    public required string URL { get; set; }
    public required string DatabaseSystem { get; set; }
    public required string ServiceName { get; set; }
    public required string Version { get; set; }
    public required string Environment { get; set; }
    public required string MachineName { get; set; }
    public required string OSVersion { get; set; }
    public required bool IsDebugLogLevelEnabled { get; set; }
    public required int ProcessId { get; set; }
    public required TimeSpan Uptime { get; set; }
}
