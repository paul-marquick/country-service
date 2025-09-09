using System.Collections.Generic;

namespace CountryService.Mappers.Country;

/// <summary>
/// Provides methods for mapping country lookup data between the domain model and the data transfer object (DTO)
/// representations.
/// </summary>
/// <remarks>This interface defines methods to convert individual country lookup entities and lists of entities
/// from the domain model to their corresponding DTO representations.</remarks>
public interface ICountryLookupMapper
{
    Dtos.Country.CountryLookup MapModelToDto(Models.Country.CountryLookup countryLookup);

    List<Dtos.Country.CountryLookup> MapModelListToDtoList(List<Models.Country.CountryLookup> countryLookupList);
}
