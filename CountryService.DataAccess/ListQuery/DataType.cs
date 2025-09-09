namespace CountryService.DataAccess.ListQuery;

/// <summary>
/// Represents the types of data that can be processed or categorized.
/// </summary>
/// <remarks>This enumeration is commonly used to specify the expected data type for validation, processing, or
/// storage purposes.</remarks>
public enum DataType
{
    Boolean,
    Text,
    Numeric,
    DateTime,
    Guid
}
