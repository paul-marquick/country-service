namespace CountryService.DataAccess.ListQuery;

/// <summary>
/// Defines a contract for determining whether a comparison operator is valid for a specific data type.
/// </summary>
/// <remarks>This interface is typically used to validate or enforce compatibility between comparison operators
/// and data types in scenarios such as query generation or data validation.</remarks>
public interface IComparisonOperatorDbType
{
    bool IsComparisonOperatorForDataType(string comparisonOperator, DataType dataType);
}
