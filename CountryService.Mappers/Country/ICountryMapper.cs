using System.Collections.Generic;

namespace CountryService.Mappers.Country;

/// <summary>
/// Provides methods for mapping between country domain models and data transfer objects (DTOs).
/// </summary>
/// <remarks>This interface defines operations for converting country entities between their domain model 
/// representation and DTO representation, as well as updating domain models with data from DTOs.</remarks>
public interface ICountryMapper
{
    Dtos.Country.Country MapModelToDto(Models.Country.Country country);
    List<Dtos.Country.Country> MapModelListToDtoList(List<Models.Country.Country> countryList);
    void UpdateModelWithDto(Models.Country.Country country, Dtos.Country.Country countryDto);
}
