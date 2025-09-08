using System.Collections.Generic;
using System.Threading.Tasks;
using CountryService.Dtos.Country;

namespace CountryService.BlazorAdminApp.HttpClients;

public interface ICountryHttpClient
{
    Task<(int, List<Country>)> GetCountriesAsync(int? offset = null, int? limit = null);
    Task<Country> GetCountryByIso2Async(string iso2);
    Task<Country> PostCountryAsync(Country country);
    Task PutCountryByIso2Async(string iso2, Country country);
    Task DeleteCountryByIso2Async(string iso2);
    Task<List<CountryLookup>> GetCountryLookupsAsync();
}
