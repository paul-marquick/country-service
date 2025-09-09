using System;
using Microsoft.Extensions.Logging;

namespace CountryService.DataAccess.ListQuery;

/// <summary>
/// Provides functionality to convert a sort direction into its corresponding SQL representation.
/// </summary>
/// <remarks>This class is used to map sort direction values, such as "Ascending" or "Descending", to their SQL
/// equivalents ("ASC" or "DESC"). It ensures that only valid sort directions are processed.</remarks>
/// <param name="logger"></param>
public class SortDirectionConverter(ILogger<SortDirectionConverter> logger) : ISortDirectionConverter
{
    public string GetSortDirectionSql(string sortDirection)
    {
        logger.LogDebug($"GetSortDirectionSql, sortDirection: {sortDirection}.");

        switch (sortDirection)
        {
            case SortDirection.Ascending:
                return "ASC";

            case SortDirection.Descending:
                return "DESC";

            default:
                throw new ArgumentException($"Unknown sort direction: {sortDirection}.");
        }
    }
}
