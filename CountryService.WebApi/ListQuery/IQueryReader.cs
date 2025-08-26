using CountryService.DataAccess.ListQuery;
using Microsoft.Extensions.Logging;

namespace CountryService.WebApi.ListQuery;

public interface IQueryReader
{
    void GetFilters(Query query, string[]? filters);
    void GetSorts(Query query, string[]? sorts, string defaultSortPropertyName, string defaultSortDirection);
}
