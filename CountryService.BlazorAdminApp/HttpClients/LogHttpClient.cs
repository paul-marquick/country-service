using CountryService.BlazorAdminApp.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CountryService.BlazorAdminApp.HttpClients;

public class LogHttpClient(HttpClient httpClient) : ILogHttpClient
{
    private const string path = "log";

    public async Task PostLogAsync(LogEntry logEntry)
    {
       var response = await httpClient.PostAsJsonAsync($"{path}", logEntry);
       //  response.EnsureSuccessStatusCode();
    }
}
