using CountryService.BlazorAdminApp.Models;

namespace CountryService.BlazorAdminApp.HttpClients;

public interface ICountryHttpClient
{
    Task<List<Country>> GetCountriesAsync();
    Task<Country> GetCountryByIso2Async(string iso2);
//    Task<Country> PostCountryAsync(Country country);
//    Task PutCountryByIso2Async(string iso2, Country country);
 //   Task DeleteCountryByIso2Async(string iso2);
}
