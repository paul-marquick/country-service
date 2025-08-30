using CountryService.BlazorAdminApp.Startup;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace CountryService.BlazorAdminApp;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");


        //TODO: add web api url to appsettings.json in wwwroot.


        builder.AddCountryServiceHttpClients("http://localhost:5581");

        builder.Services.AddBlazorBootstrap();

        await builder.Build().RunAsync();
    }
}

// Add UseAntiForgery in Program.cs
