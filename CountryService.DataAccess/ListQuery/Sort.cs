namespace CountryService.DataAccess.ListQuery;

public record Sort
{
    public Sort(string propertyName, SortDirection sortDirection)
    {
        PropertyName = propertyName;
        SortDirection = sortDirection;
    }

    public string PropertyName { get; set; }
    public SortDirection SortDirection { get; set; }
}