using System.Threading.Tasks;
using CountryService.BlazorAdminApp.Models;

namespace CountryService.BlazorAdminApp.HttpClients;

public interface ILogHttpClient
{
    Task PostLogAsync(LogEntry logEntry);
}
