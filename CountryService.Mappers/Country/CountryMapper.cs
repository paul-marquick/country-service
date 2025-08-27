using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace CountryService.Mappers.Country;

public class CountryMapper(ILogger<CountryMapper> logger) : ICountryMapper
{
    public Dtos.Country.Country MapModelToDto(Models.Country.Country country)
    {
        logger.LogDebug("CountryMapper.MapModelToDto.");

        return new Dtos.Country.Country
        {
            Iso2 = country.Iso2,
            Iso3 = country.Iso3,
            IsoNumber = country.IsoNumber,
            Name = country.Name,
            CallingCode = country.CallingCode,
        };
    }

    public List<Dtos.Country.Country> MapModelListToDtoList(List<Models.Country.Country> countryList)
    {
        logger.LogDebug("CountryMapper.MapModelListToDtoList.");

        List<Dtos.Country.Country> countryDtoList = new List<Dtos.Country.Country>();

        foreach (Models.Country.Country country in countryList)
        {
            countryDtoList.Add(MapModelToDto(country));
        }

        return countryDtoList;
    }

    public void UpdateModelWithDto(Models.Country.Country country, Dtos.Country.Country countryDto)
    {
        logger.LogDebug("CountryMapper.UpdateModelWithDto.");

        country.Iso2 = countryDto.Iso2!;
        country.Iso3 = countryDto.Iso3!;
        country.IsoNumber = countryDto.IsoNumber!.Value;
        country.Name = countryDto.Name!;
        country.CallingCode = countryDto.CallingCode;
    }
}
