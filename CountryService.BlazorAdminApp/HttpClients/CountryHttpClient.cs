using CountryService.BlazorAdminApp.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CountryService.BlazorAdminApp.HttpClients;

public class CountryHttpClient(HttpClient httpClient) : ICountryHttpClient
{
    private const string path = "country";

    public async Task<List<Country>> GetCountriesAsync()
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path);

        HttpResponseMessage response = await httpClient.SendAsync(request);

        //   response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Country>>();
    }

    public async Task<Country> GetCountryByIso2Async(string iso2)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{path}/{iso2}");

        Console.WriteLine($"Request: {request.Method} {request.RequestUri}");

        HttpResponseMessage response = await httpClient.SendAsync(request);
     //   await response.CheckStatusAsync();

        return await response.Content.ReadFromJsonAsync<Country>();

    //    var response = await httpClient.GetAsync($"{url}/{iso2}");
    //    response.EnsureSuccessStatusCode();

     //   return await response.Content.ReadFromJsonAsync<Country>();
    }

    //public async Task<Country> PostCountryAsync(Country country)
    //{
    //    var response = await httpClient.PostAsJsonAsync(url, country);
    //    //  response.EnsureSuccessStatusCode();

    //    return await response.Content.ReadFromJsonAsync<Country>();
    //}

    //public async Task PutCountryByIso2Async(string iso2, Country country)
    //{
    //    var response = await httpClient.PutAsJsonAsync($"{url}/{iso2}", country);

    //    //response.EnsureSuccessStatusCode();
    //}

    //public async Task DeleteCountryByIso2Async(string iso2)
    //{
    //    var response = await httpClient.DeleteAsync($"{url}/{iso2}");
        
    //    // response.EnsureSuccessStatusCode();
    //}
}
