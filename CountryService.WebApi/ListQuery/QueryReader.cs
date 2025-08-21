using CountryService.DataAccess.ListQuery;

namespace CountryService.WebApi.ListQuery;

public class QueryReader(ILogger<QueryReader> logger)
{
    public void GetFilters(Query query, string[]? filters)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("ValidateFilters.");

            if (filters == null)
            {
                logger.LogDebug("filters: null");
            }
            else
            {
                logger.LogDebug($"filters: {string.Join(", ", filters)}");
            }
        }

        if (filters != null && filters.Length > 0)
        {
            foreach (string f in filters)
            {
                // Example filter - name:like:fred

                // Split on colon.
                string[] filterParts = f.Split(':');
                string propertyName = filterParts[0];
                string comparisonOperator = filterParts[1];
                string value = string.Join(':', filterParts, 2, filterParts.Length - 1);

                query.AddFilter(propertyName, comparisonOperator, value);
            }
        }
    }

    public void GetSorts(Query query, string[]? sorts, string defaultSortPropertyName, string defaultSortDirection)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug($"ValidateSorts, defaultSortPropertyName: {defaultSortPropertyName}, defaultSortDirection: {defaultSortDirection}.");

            if (sorts == null)
            {
                logger.LogDebug("sorts: null");
            }
            else
            {
                logger.LogDebug($"sorts: {string.Join(", ", sorts)}");
            }
        }

        if (sorts == null || sorts.Length == 0)
        {
            logger.LogDebug("Default sort applied.");

            query.AddSort(defaultSortPropertyName, defaultSortDirection);
        }
        else
        {
            foreach (string s in sorts)
            {
                // Example: iso2:desc

                // Split on colon.
                string[] sortParts = s.Split(':');
                string propertyName = sortParts[0];
                string sortDirection = sortParts[1];

                query.AddSort(propertyName, sortDirection);
            }
        }
    }
}
