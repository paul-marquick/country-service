using CountryService.BlazorAdminApp.HttpClients;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CountryService.BlazorAdminApp.Startup;

public static class HttpClientAdder
{
    public static void AddCountryServiceHttpClients(this WebAssemblyHostBuilder builder, string webApiBaseAddress)
    {
        builder.Services.AddHttpClient<ILogHttpClient, LogHttpClient>(client =>
        {
            client.BaseAddress = new Uri(webApiBaseAddress);
        });

        builder.Services.AddHttpClient<ICountryHttpClient, CountryHttpClient>(client =>
        {
            client.BaseAddress = new Uri(webApiBaseAddress);
        });

        builder.Services.AddHttpClient<IServiceInfoHttpClient, ServiceInfoHttpClient>(client =>
        {
            client.BaseAddress = new Uri(webApiBaseAddress);
        });
    }
}
