namespace CountryService.DataAccess.ListQuery;

public record Query
{
    public Query(int offset, int limit)
    {
        OffSet = offset;
        Limit = limit;
    }

    public int OffSet { get; set; }
    public int Limit { get; set; }
    public List<Filter>? Filters { get; set; }
    public List<Sort>? Sorts { get; set; }

    public void AddFilter(string propertyName, ComparisonOperator comparisonOperator, object value)
    {
        if (Filters == null)
        {
            Filters = new List<Filter>();
        }

        Filters.Add(new Filter(propertyName, comparisonOperator, value));
    }

    public void AddSort(string propertyName, SortDirection sortDirection)
    {
        if (Sorts == null)
        {
            Sorts = new List<Sort>();
        }

        Sorts.Add(new Sort(propertyName, sortDirection));
    }
}
