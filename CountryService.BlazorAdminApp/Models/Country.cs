namespace CountryService.BlazorAdminApp.Models;

public class Country
{
    public string Iso2 { get; set; }
    public string Iso3 { get; set; }
    public int IsoNumber { get; set; }
    public string Name { get; set; }
    public string? CallingCode { get; set; }
}
