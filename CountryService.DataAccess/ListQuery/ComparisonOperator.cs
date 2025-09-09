namespace CountryService.DataAccess.ListQuery;

/// <summary>
/// Provides a set of string constants representing common comparison operators.
/// </summary>
/// <remarks>These constants are typically used to represent comparison operations in contexts such as query
/// building, filtering, or other scenarios where comparison logic is expressed as strings.</remarks>
public static class ComparisonOperator
{
    public const string EqualTo = "e";
    public const string NotEqualTo = "ne";
    public const string GreaterThan = "g";
    public const string LessThan = "l";
    public const string GreaterThanOrEqualTo = "ge";
    public const string LessThanOrEqualTo = "le";
    public const string Like = "like";
}
