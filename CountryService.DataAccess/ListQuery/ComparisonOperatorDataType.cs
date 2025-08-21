using Microsoft.Extensions.Logging;

namespace CountryService.DataAccess.ListQuery;

public class ComparisonOperatorDbType(ILogger<ComparisonOperatorDbType> logger)
{
    public bool IsComparisonOperatorForDataType(string comparisonOperator, DataType dataType)
    {
        logger.LogDebug($"IsComparisonOperatorForDataType, comparisonOperator: {comparisonOperator}, dataType: {dataType}.");

        switch (comparisonOperator.ToLower())
        {
            case ComparisonOperator.EqualTo:
            case ComparisonOperator.NotEqualTo:
                return dataTypesForEqualAndNotEqual.Contains(dataType);

            case ComparisonOperator.GreaterThan:
            case ComparisonOperator.LessThan:
            case ComparisonOperator.GreaterThanOrEqualTo:
            case ComparisonOperator.LessThanOrEqualTo:
                return dataTypesForInEquality.Contains(dataType);

            case ComparisonOperator.Like:
                return dataTypesForLike.Contains(dataType);

            default:
                throw new ArgumentException($"Unknown comparison operator : {comparisonOperator}");
        }
    }

    private static readonly DataType[] dataTypesForEqualAndNotEqual =
    {
        DataType.Boolean,
        DataType.Text,
        DataType.Numeric,
        DataType.Guid,
        DataType.DateTime
    };

    private static readonly DataType[] dataTypesForInEquality =
    {
        DataType.DateTime,
        DataType.Numeric,
        DataType.Guid
    };

    private static readonly DataType[] dataTypesForLike =
    {
        DataType.Text
    };
}
