namespace CountryService.DataAccess;

public record Country
{
    public required string Iso2 { get; set; }
    public required string Iso3 { get; set; }
    public int IsoNumber { get; set; }
    public required string Name { get; set; }
}
