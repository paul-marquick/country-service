using CountryService.WebApi.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CountryService.WebApi.Startup;

public static class ConfigAdder
{
    public static void AddConfig(this WebApplicationBuilder builder)
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
