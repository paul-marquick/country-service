using System.Collections.Generic;

namespace CountryService.BlazorAdminApp.Problems;

public record ExtendedValidationProblemDetails
{
    public int? Status { get; set; }

    public string? Type { get; set; }

    public string? Title { get; set; }

    public string? Detail { get; set; }

    public string? Instance { get; set; }

    public string? RequestId { get; set; }

    public string? CorrelationId { get; set; }

    public IDictionary<string, string[]>? Errors { get; set; }
}
