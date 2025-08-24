using System;
using Microsoft.Extensions.Logging;

namespace CountryService.DataAccess.ListQuery;

public class SortDirectionConverter(ILogger<SortDirectionConverter> logger)
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
