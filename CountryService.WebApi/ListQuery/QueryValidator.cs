using System;
using System.Linq;
using CountryService.DataAccess.ListQuery;
using CountryService.Shared;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace CountryService.WebApi.ListQuery;

public class QueryValidator(ILogger<QueryValidator> logger, ComparisonOperatorDbType comparisonOperatorDbType)
{
    public void Validate(
        ModelStateDictionary modelState,
        string[] filterableProperties,
        string[] SortableProperties,
        Func<string, DataType> getDataType,
        int offset,
        int limit,
        string[]? filters,
        string[]? sorts)
    {
        logger.LogDebug("ValidateQuery");

        ValidateOffset(modelState, offset);
        ValidateLimit(modelState, limit);
        ValidateFilters(modelState, filterableProperties, getDataType, filters);
        ValidateSorts(modelState, SortableProperties, sorts);
    }

    private void ValidateOffset(ModelStateDictionary modelState, int offset)
    {
        logger.LogDebug($"ValidateOffset, offset: {offset}");

        if (offset < 0)
        {
            modelState.AddModelError(PropertyNames.Offset, "offset must be greater or equal to 0.");
        }
    }

    private void ValidateLimit(ModelStateDictionary modelState, int limit)
    {
        logger.LogDebug($"ValidateLimit, limit: {limit}");

        if (!Constants.ValidLimits.Contains(limit))
        {
            modelState.AddModelError(PropertyNames.Limit, "limit must be 10, 20, 50 or 100.");
        }
    }
    
    private void ValidateFilters(
        ModelStateDictionary modelState,
        string[] filterableProperties, 
        Func<string, DataType> getDataType,
        string[]? filters)
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

                // Check the property name is filterable.
                if (!filterableProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase))
                {
                    modelState.AddModelError(PropertyNames.Filters, "Property is invalid.");
                }

                // Check the comparison operator is valid.
                if (!comparisonOperator.Equals(ComparisonOperator.EqualTo, StringComparison.OrdinalIgnoreCase) &&
                    !comparisonOperator.Equals(ComparisonOperator.NotEqualTo, StringComparison.OrdinalIgnoreCase) &&
                    !comparisonOperator.Equals(ComparisonOperator.GreaterThan, StringComparison.OrdinalIgnoreCase) &&
                    !comparisonOperator.Equals(ComparisonOperator.LessThan, StringComparison.OrdinalIgnoreCase) &&
                    !comparisonOperator.Equals(ComparisonOperator.GreaterThanOrEqualTo, StringComparison.OrdinalIgnoreCase) &&
                    !comparisonOperator.Equals(ComparisonOperator.LessThanOrEqualTo, StringComparison.OrdinalIgnoreCase) &&
                    !comparisonOperator.Equals(ComparisonOperator.Like, StringComparison.OrdinalIgnoreCase))
                {
                    modelState.AddModelError(PropertyNames.Filters, "Comparison operator is invalid.");
                }
                else
                {
                    // Check the property data type and comparison operator are compatible.
                    if (!comparisonOperatorDbType.IsComparisonOperatorForDataType(comparisonOperator, getDataType(propertyName)))
                    {
                        modelState.AddModelError(PropertyNames.Filters, "Comparison operator is not compatible with the data type of the specified property.");
                    }
                }
            }
        }
    }

    private void ValidateSorts(ModelStateDictionary modelState, string[] sortableProperties, string[]? sorts)
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

        if (sorts != null && sorts.Length > 0)
        {
            foreach (string s in sorts)
            {
                // Example: iso2:desc

                // Split on colon.
                string[] sortParts = s.Split(':');
                string propertyName = sortParts[0];
                string sortDirection = sortParts[1];

                // Check the property name is sortable.
                if (!sortableProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase))
                {
                    modelState.AddModelError(PropertyNames.Sorts, "Property is invalid.");
                }

                // Check the sort direction is valid.
                if (!sortDirection.Equals(SortDirection.Ascending, StringComparison.OrdinalIgnoreCase) &&
                    !sortDirection.Equals(SortDirection.Descending, StringComparison.OrdinalIgnoreCase))
                {
                    modelState.AddModelError(PropertyNames.Sorts, "Sort direction is invalid.");
                }
            }
        }
    }
}
