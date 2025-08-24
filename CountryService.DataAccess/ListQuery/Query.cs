using System.Collections.Generic;

namespace CountryService.DataAccess.ListQuery;

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
