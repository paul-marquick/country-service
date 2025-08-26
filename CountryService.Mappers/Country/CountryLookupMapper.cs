using System.Collections.Generic;

namespace CountryService.Mappers.Country;

public class CountryLookupMapper : ICountryLookupMapper
{
    public Dtos.Country.CountryLookup MapModelToDto(Models.Country.CountryLookup countryLookup)
    {
        return new Dtos.Country.CountryLookup
        {
            Iso2 = countryLookup.Iso2,
            Name = countryLookup.Name
        };
    }

    public List<Dtos.Country.CountryLookup> MapModelListToDtoList(List<Models.Country.CountryLookup> countryLookupList)
    {
        List<Dtos.Country.CountryLookup> countryLookupsDtoList = new List<Dtos.Country.CountryLookup>();

        foreach (Models.Country.CountryLookup countryLookup in countryLookupList)
        {
            countryLookupsDtoList.Add(MapModelToDto(countryLookup));
        }

        return countryLookupsDtoList;
    }
}
