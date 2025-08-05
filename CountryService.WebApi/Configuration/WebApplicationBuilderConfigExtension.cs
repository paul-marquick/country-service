namespace CountryService.WebApi.Configuration;

public static class WebApplicationBuilderConfigExtension
{
    public static void AddAppSettings(this WebApplicationBuilder builder)
    {
        const string appSettingsFilename = "appsettings";

        builder.Configuration.AddJsonFile($"{appSettingsFilename}.json", false, true);
        builder.Configuration.AddJsonFile($"{appSettingsFilename}.{builder.Environment.EnvironmentName}.json", true, true);
    }

    public static Config GetConfig(this WebApplicationBuilder builder)
    {
        const string configSectionName = "Config";

        IConfigurationSection configurationSection = builder.Configuration.GetSection(configSectionName);
        builder.Services.Configure<Config>(configurationSection);

        return configurationSection.Get<Config>()!;
    }
}
