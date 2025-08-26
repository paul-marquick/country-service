using CountryService.Dtos.ServiceInfo;
using CountryService.Shared;
using System.Net.Http;
using System.Threading.Tasks;

namespace CountryService.BlazorAdminApp.HttpClients;

public class ServiceInfoHttpClient(HttpClient httpClient) : IServiceInfoHttpClient
{
    public async Task<ServiceInfo> GetServiceInfoAsync()
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Paths.WebApi.ServiceInfo.BasePath);

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        await response.CheckStatusAsync();

        return await response.GetJsonDataAsync<ServiceInfo>();
    }    
}
