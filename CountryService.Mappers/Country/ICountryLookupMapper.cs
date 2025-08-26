using System.Collections.Generic;

namespace CountryService.Mappers.Country;

public interface ICountryLookupMapper
{
    Dtos.Country.CountryLookup MapModelToDto(Models.Country.CountryLookup countryLookup);

    List<Dtos.Country.CountryLookup> MapModelListToDtoList(List<Models.Country.CountryLookup> countryLookupList);
}
