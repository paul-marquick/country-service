using CountryService.Dtos.Country;
using CountryService.Shared;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CountryService.BlazorAdminApp.HttpClients;

public class CountryHttpClient(HttpClient httpClient) : ICountryHttpClient
{
    public async Task<List<Country>> GetCountriesAsync()
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Paths.WebApi.Country.BasePath);

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        await response.CheckStatusAsync();

        return await response.GetJsonDataAsync<List<Country>>();
    }

    public async Task<Country> GetCountryByIso2Async(string iso2)
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{Paths.WebApi.Country.BasePath}/{iso2}");

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        await response.CheckStatusAsync();

        return await response.GetJsonDataAsync<Country>();
    }

    public async Task<Country> PostCountryAsync(Country country)
    {        
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Paths.WebApi.Country.BasePath);
        request.AddJsonData(country);

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        await response.CheckStatusAsync();

        return await response.GetJsonDataAsync<Country>();       
    }

    public async Task PutCountryByIso2Async(string iso2, Country country)
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{Paths.WebApi.Country.BasePath}/{iso2}");
        request.AddJsonData(country);

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        await response.CheckStatusAsync();
    }

    public async Task DeleteCountryByIso2Async(string iso2)
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{Paths.WebApi.Country.BasePath}/{iso2}");

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        await response.CheckStatusAsync();
    }

    public async Task<List<CountryLookup>> GetCountryLookupsAsync()
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{Paths.WebApi.Country.BasePath}/lookup");

        using HttpResponseMessage response = await httpClient.SendAsync(request);
        await response.CheckStatusAsync();

        return await response.GetJsonDataAsync<List<CountryLookup>>();
    }
}
