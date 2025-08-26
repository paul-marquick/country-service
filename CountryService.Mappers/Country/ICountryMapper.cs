using System.Collections.Generic;

namespace CountryService.Mappers.Country;

public interface ICountryMapper
{
    Dtos.Country.Country MapModelToDto(Models.Country.Country country);
    List<Dtos.Country.Country> MapModelListToDtoList(List<Models.Country.Country> countryList);
    void UpdateModelWithDto(Models.Country.Country country, Dtos.Country.Country countryDto);
}
