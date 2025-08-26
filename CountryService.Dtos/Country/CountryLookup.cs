namespace CountryService.Dtos.Country;

public record CountryLookup
{
    public required string Iso2 { get; set; }
    public required string Name { get; set; }
}
