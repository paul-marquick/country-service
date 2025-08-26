namespace CountryService.DataAccess.ListQuery;

public interface IComparisonOperatorDbType
{
    bool IsComparisonOperatorForDataType(string comparisonOperator, DataType dataType);
}
