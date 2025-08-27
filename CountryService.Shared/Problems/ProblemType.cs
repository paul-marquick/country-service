namespace CountryService.Shared.Problems;

/// <summary>
/// The "type" member is a JSON string containing a URI reference that identifies the problem type
/// </summary>
/// <see cref="https://www.rfc-editor.org/rfc/rfc9457.html#name-type"/>
/// <remarks>Would have a CMS docs site, where these could be dereferenced for more detail.</remarks>
public static class ProblemType
{
    // General problem types.
    public const string FailedValidation = "failed-validation";
    public const string InternalServerError = "internal-server-error";
    public const string UnsupportedPatchOperation = "unsupported-patch-operation";

    // Country resource specific problem types.
    public const string CountryNotFound = "country-not-found";
    public const string CountryIso2Duplicated = "country-iso2-duplicated";
    public const string CountryIso3Duplicated = "country-iso3-duplicated";
    public const string CountryIsoNumberDuplicated = "country-isoNumber-duplicated";
    public const string CountryNameDuplicated = "country-name-duplicated";
}
