using System.Threading.Tasks;
using CountryService.Dtos.ServiceInfo;

namespace CountryService.BlazorAdminApp.HttpClients;

public interface IServiceInfoHttpClient
{
    Task<ServiceInfo> GetServiceInfoAsync();
}
