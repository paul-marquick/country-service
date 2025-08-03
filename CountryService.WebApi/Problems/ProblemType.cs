namespace CountryService.WebApi.Problems;

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

    // Country resource specific problem types.
    public const string CountryNotFound = "country-not-found";
    public const string CountryIso2Duplicated = "country-iso2-duplicated";
    public const string CountryIso3Duplicated = "country-iso3-duplicated";
    public const string CountryIsoNumberDuplicated = "country-isoNumber-duplicated";
    public const string CountryNameDuplicated = "country-name-duplicated";

    // Added some extra possible problem types as examples.
    public const string UserNotFound = "user-not-found";
    public const string UsernameDuplicated = "username-duplicated";
    public const string EmailAddressDuplicated = "email-address-duplicated";
    public const string PasswordInvalid = "password-invalid";
    public const string UserLockedOut = "user-locked-out";
    public const string UserTemporarilyLockedOut = "user-temporarily-locked-out";
    public const string ScopeForbidden = "scope-forbidden";
    public const string ScopeConfidentialClientOnly = "scope-confidential-client-only";
    public const string ResponseTypeInvalid = "response-type-invalid";
    public const string ScopeInvalid = "scope-invalid";
    public const string CodeChallengeEmpty = "code-challenge-empty";
    public const string UserEmailAddressNotVerified = "user-email-address-not-verified";
}
