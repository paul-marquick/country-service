using System.Collections.Generic;

namespace CountryService.DataAccess.ListQuery;

/// <summary>
/// Represents a query with pagination, filtering, and sorting capabilities.
/// </summary>
/// <remarks>This type is used to construct and manage queries that include support for pagination through the
/// <see cref="OffSet"/> and <see cref="Limit"/> properties, as well as optional filtering and sorting through the <see
/// cref="Filters"/> and <see cref="Sorts"/> collections. Filters and sorts can be added dynamically using the <see
/// cref="AddFilter"/> and <see cref="AddSort"/> methods.</remarks>
public record Query
{
    public Query(int offset, int limit)
    {
        OffSet = offset;
        Limit = limit;
        Sorts = new List<Sort>();        
    }

    public int OffSet { get; set; }
    public int Limit { get; set; }
    public List<Filter>? Filters { get; set; }
    public List<Sort> Sorts { get; set; }

    public void AddFilter(string propertyName, string comparisonOperator, object value)
    {
        if (Filters == null)
        {
            Filters = new List<Filter>();
        }

        Filters.Add(new Filter(propertyName, comparisonOperator, value));
    }

    public void AddSort(string propertyName, string sortDirection)
    {
        Sorts.Add(new Sort(propertyName, sortDirection));
    }
}
