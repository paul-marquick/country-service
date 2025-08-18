namespace CountryService.DataAccess.ListQuery;

public record Filter
{
    public Filter(string propertyName, ComparisonOperator comparisonOperator, object value)
    {
        PropertyName = propertyName;
        ComparisonOperator = comparisonOperator;
        Value = value;
    }

    public string PropertyName { get; set; }
    public ComparisonOperator ComparisonOperator { get; set; }
    public object Value { get; set; }
}
