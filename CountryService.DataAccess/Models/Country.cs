using System.ComponentModel.DataAnnotations;

namespace CountryService.DataAccess.Models;

public record Country
{
    [Required, MinLength(2), MaxLength(2)]
    public required string Iso2 { get; set; }

    [Required, MinLength(3), MaxLength(3)]
    public required string Iso3 { get; set; }

    [Required]
    public int IsoNumber { get; set; }

    [Required]
    public required string Name { get; set; }
}
