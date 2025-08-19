namespace CountryService.DataAccess.ListQuery;

public static class ComparisonOperatorConverter
{
    public static string GetComparisonOperatorSql(string comparisonOperator)
    {
        switch (comparisonOperator)
        {
            case ComparisonOperator.EqualTo:
                return "=";

            case ComparisonOperator.NotEqualTo:
                return "<>";

            case ComparisonOperator.GreaterThan:
                return ">";

            case ComparisonOperator.LessThan:
                return "<";

            case ComparisonOperator.GreaterThanOrEqualTo:
                return ">=";

            case ComparisonOperator.LessThanOrEqualTo:
                return "<=";

            case ComparisonOperator.Like:
                return "LIKE";

            default:
                throw new ArgumentException($"Unknown comparison operator: {comparisonOperator}.");
        }
    }
}
