using CountryService.BlazorAdminApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CountryService.BlazorAdminApp.HttpClients;

public class CountryHttpClient(HttpClient httpClient) : ICountryHttpClient
{
    private const string path = "country";

    public async Task<List<Country>> GetCountriesAsync()
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path);

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        await response.CheckStatusAsync();

        return await response.GetJsonDataAsync<List<Country>>();
    }

    public async Task<Country> GetCountryByIso2Async(string iso2)
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{path}/{iso2}");

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        await response.CheckStatusAsync();

        return await response.GetJsonDataAsync<Country>();
    }

    public async Task<Country> PostCountryAsync(Country country)
    {        
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, path);
        request.AddJsonData(country);

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        await response.CheckStatusAsync();

        return await response.GetJsonDataAsync<Country>();       
    }

    public async Task PutCountryByIso2Async(string iso2, Country country)
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{path}/{iso2}");
        request.AddJsonData(country);

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        await response.CheckStatusAsync();
    }

    public async Task DeleteCountryByIso2Async(string iso2)
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{path}/{iso2}");

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        await response.CheckStatusAsync();
    }

    // public async Task<List<CountryLookup>> GetCountryLookupsAsync()
    // {
    //     using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path);

    //     using HttpResponseMessage response = await httpClient.SendAsync(request);
    //     await response.CheckStatusAsync();

    //     return await response.GetJsonDataAsync<List<Country>>();
    // }
}
