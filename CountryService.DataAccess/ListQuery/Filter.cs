namespace CountryService.DataAccess.ListQuery;

/// <summary>
/// Represents a filter condition used to evaluate whether a property satisfies a specified comparison.
/// </summary>
/// <remarks>A filter consists of a property name, a comparison operator, and a value to compare against.  It can
/// be used to define criteria for querying or filtering data in various contexts.</remarks>
public record Filter
{
    public Filter(string propertyName, string comparisonOperator, object value)
    {
        PropertyName = propertyName;
        ComparisonOperator = comparisonOperator;
        Value = value;
    }

    public string PropertyName { get; set; }
    public string ComparisonOperator { get; set; }
    public object Value { get; set; }
}
