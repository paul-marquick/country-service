using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CountryService.BlazorAdminApp.Startup;

public static class ConfigGetting
{
    public static Configuration.Config GetConfig(this WebAssemblyHostBuilder builder)
    {
        const string configSectionName = "Config";

        IConfigurationSection configurationSection = builder.Configuration.GetSection(configSectionName);
        builder.Services.Configure<Configuration.Config>(configurationSection);

        return configurationSection.Get<Configuration.Config>()!;
    }
}
