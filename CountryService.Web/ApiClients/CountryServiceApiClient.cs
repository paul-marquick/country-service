using CountryService.Web.ApiClients.Models;

namespace CountryService.Web.ApiClients;

public class CountryServiceApiClient(HttpClient httpClient)
{
    public async Task<Country[]> GetCountryListAsync(CancellationToken cancellationToken = default)
    {
        List<Country>? countryList = null;

        await foreach (var country in httpClient.GetFromJsonAsAsyncEnumerable<Country>("/country", cancellationToken))
        {
            if (country is not null)
            {
                countryList ??= [];
                countryList.Add(country);
            }
        }

        return countryList?.ToArray() ?? [];
    }

    public async Task<Country?> GetCountryByIso2Async(string iso2, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<Country?>($"/country/{iso2}", cancellationToken);
    }
}
