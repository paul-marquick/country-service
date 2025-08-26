using CountryService.Dtos.Log;
using CountryService.Shared;
using System.Net.Http;
using System.Threading.Tasks;

namespace CountryService.BlazorAdminApp.HttpClients;

public class LogHttpClient(HttpClient httpClient) : ILogHttpClient
{
    public async Task PostLogAsync(Log log)
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Paths.WebApi.Log.BasePath);
        request.AddJsonData(log);

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        await response.CheckStatusAsync();
    }    
}
