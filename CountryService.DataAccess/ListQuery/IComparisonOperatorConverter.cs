namespace CountryService.DataAccess.ListQuery;

/// <summary>
/// Defines a contract for converting comparison operators into their SQL representation.
/// </summary>
/// <remarks>This interface is typically used to map logical or symbolic comparison operators  (e.g., "Equals",
/// "GreaterThan") to their corresponding SQL syntax (e.g., "=", ">")  for use in dynamically generated SQL
/// queries.</remarks>
public interface IComparisonOperatorConverter
{
    string GetComparisonOperatorSql(string comparisonOperator);
}
