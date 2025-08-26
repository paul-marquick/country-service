using System.ComponentModel.DataAnnotations;

namespace CountryService.Dtos.Country;

public record Country
{
    [Required(ErrorMessage = "Required"), MinLength(2, ErrorMessage = "Too short, min length is 2."), MaxLength(2, ErrorMessage = "Too long, max length is 2.")]
    public required string Iso2 { get; set; }

    [Required(ErrorMessage = "Required"), MinLength(3, ErrorMessage = "Too short, min length is 3."), MaxLength(3, ErrorMessage = "Too long, max length is 3.")]
    public required string Iso3 { get; set; }

    [Required(ErrorMessage = "Required")]
    public int IsoNumber { get; set; }

    [Required(ErrorMessage = "Required")]
    public required string Name { get; set; }

    public string? CallingCode { get; set; }
}
