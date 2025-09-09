namespace CountryService.WebApi.Configuration;

/// <summary>
/// Represents the configuration settings for the application, including database and API details.
/// </summary>
/// <remarks>This record is used to store essential configuration values such as the database system type and the
/// base URL for the web API. All properties are required and must be initialized.</remarks>
public record Config
{
    public required string DatabaseSystem { get; set; }
    public required string WebApiUrl { get; set; }
}
