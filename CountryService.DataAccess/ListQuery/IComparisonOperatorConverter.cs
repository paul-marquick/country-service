namespace CountryService.DataAccess.ListQuery;

public interface IComparisonOperatorConverter
{
    string GetComparisonOperatorSql(string comparisonOperator);
}
