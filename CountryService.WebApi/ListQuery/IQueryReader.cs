using CountryService.DataAccess.ListQuery;

namespace CountryService.WebApi.ListQuery;

/// <summary>
/// Defines methods for extracting filter and sort criteria from strings.
/// </summary>
/// <remarks>This interface is designed to provide a standardized way to retrieve filtering and sorting
/// information from strings.</remarks>
public interface IQueryReader
{
    void GetFilters(Query query, string[]? filters);
    void GetSorts(Query query, string[]? sorts, string defaultSortPropertyName, string defaultSortDirection);
}
