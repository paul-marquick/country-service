using CountryService.WebApi.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CountryService.WebApi.Startup;

public static class ConfigGetting
{
    public static Config GetConfig(this WebApplicationBuilder builder)
    {
        const string configSectionName = "Config";

        IConfigurationSection configurationSection = builder.Configuration.GetSection(configSectionName);
        builder.Services.Configure<Config>(configurationSection);

        return configurationSection.Get<Config>()!;
    }
}
