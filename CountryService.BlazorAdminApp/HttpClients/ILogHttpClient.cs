using System.Threading.Tasks;
using CountryService.Dtos.Log;

namespace CountryService.BlazorAdminApp.HttpClients;

public interface ILogHttpClient
{
    /// <summary>
    /// Sends application errors to the web service.
    /// </summary>
    /// <param name="log"></param>
    Task PostLogAsync(Log log);
}
