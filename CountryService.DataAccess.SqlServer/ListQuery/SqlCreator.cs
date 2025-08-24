using CountryService.DataAccess.ListQuery;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace CountryService.DataAccess.SqlServer.ListQuery;

public class SqlCreator(
    ILogger<SqlCreator> logger,
    ComparisonOperatorConverter comparisonOperatorConverter,
    SortDirectionConverter sortDirectionConverter)
{
    public string CreateQueryWhereClauseSql(List<Filter>? filters)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("CreateQueryWhereClauseSql.");

            if (filters == null)
            {
                logger.LogDebug("filters: null");
            }
            else
            {
                logger.LogDebug($"filters: {string.Join(", ", filters)}");
            }
        }

        if (filters == null)
        {
            return string.Empty;
        }
        else
        {
            string result = "WHERE ";

            for (int i = 0; filters.Count > i; i++)
            {
                result += $"\"{filters[i].PropertyName}\" {comparisonOperatorConverter.GetComparisonOperatorSql(filters[i].ComparisonOperator)} @{filters[i].PropertyName} ";

                if (i < filters.Count - 1)
                {
                    // Add logical operator, Only AND is currently supported.
                    result += "AND ";
                }
            }

            return result;
        }
    }

    public void AddQueryWhereClauseParameters(SqlCommand dbCommand, List<Filter>? filters)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("AddQueryWhereClauseParameters.");

            if (filters == null)
            {
                logger.LogDebug("filters: null");
            }
            else
            {
                logger.LogDebug($"filters: {string.Join(", ", filters)}");
            }
        }

        if (filters != null)
        {
            foreach (Filter filter in filters)
            {
                dbCommand.Parameters.AddWithValue(filter.PropertyName, filter.Value);
            }
        }
    }

    public string CreateQueryOrderByClauseSql(List<Sort> sorts)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("ValidateSorts.");

            if (sorts == null)
            {
                logger.LogDebug("sorts: null");
            }
            else
            {
                logger.LogDebug($"sorts: {string.Join(", ", sorts)}");
            }
        }

        string result = "ORDER BY ";

        for (int i = 0; sorts!.Count > i; i++)
        {
            result += $"\"{sorts[i].PropertyName}\" {sortDirectionConverter.GetSortDirectionSql(sorts[i].SortDirection)} ";

            if (i < sorts.Count - 1)
            {
                result += ", ";
            }
        }

        return result;
    }
}
