namespace CountryService.DataAccess.ListQuery;

/// <summary>
/// Represents a sorting operation, specifying the property to sort by and the direction of the sort.
/// </summary>
/// <remarks>This type is typically used to define sorting criteria for data queries or collections. The <see
/// cref="PropertyName"/> property specifies the name of the property to sort by,  and the <see cref="SortDirection"/>
/// property indicates the direction of the sort,  such as "Ascending" or "Descending".</remarks>
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
