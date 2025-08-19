namespace CountryService.DataAccess.ListQuery;

public record Sort
{
    public Sort(string propertyName, string sortDirection)
    {
        PropertyName = propertyName;
        SortDirection = sortDirection;
    }

    public string PropertyName { get; set; }
    public string SortDirection { get; set; }
}
