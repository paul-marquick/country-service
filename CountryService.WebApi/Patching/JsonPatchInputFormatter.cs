using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CountryService.WebApi.Patching;

/// <summary>
/// Provides a method to retrieve an instance of <see cref="NewtonsoftJsonPatchInputFormatter"/>  configured with
/// default services and options.
/// </summary>
/// <remarks>This method initializes a new <see cref="ServiceCollection"/> with logging and MVC services,  adds
/// support for Newtonsoft.Json, and retrieves the first instance of  <see cref="NewtonsoftJsonPatchInputFormatter"/>
/// from the configured input formatters.</remarks>
public static class JsonPatchInputFormatter
{
    public static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
    {
        ServiceProvider builder = new ServiceCollection()
            .AddLogging()
            .AddMvc()
            .AddNewtonsoftJson()
            .Services.BuildServiceProvider();

        return builder
            .GetRequiredService<IOptions<MvcOptions>>()
            .Value
            .InputFormatters
            .OfType<NewtonsoftJsonPatchInputFormatter>()
            .First();
    }
}
