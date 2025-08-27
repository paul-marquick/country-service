namespace CountryService.WebApi.Configuration;

public record Config
{
    public required string DatabaseSystem { get; set; }
}
