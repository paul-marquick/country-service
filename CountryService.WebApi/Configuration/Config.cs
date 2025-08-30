namespace CountryService.WebApi.Configuration;

public record Config
{
    public required string DatabaseSystem { get; set; }
    public required string WebApiUrl { get; set; }
}
