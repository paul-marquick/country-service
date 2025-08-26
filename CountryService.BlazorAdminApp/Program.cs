using System;
using System.Threading.Tasks;
using CountryService.BlazorAdminApp.HttpClients;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CountryService.BlazorAdminApp;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        Console.WriteLine(builder.HostEnvironment.Environment);

     //   builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        builder.Services.AddHttpClient<ILogHttpClient, LogHttpClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5581");
        });

        builder.Services.AddHttpClient<ICountryHttpClient, CountryHttpClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5581");
        });

        builder.Services.AddHttpClient<IServiceInfoHttpClient, ServiceInfoHttpClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5581");
        });

        await builder.Build().RunAsync();
    }
}

// Add UseAntiForgery in Program.cs
