using System;
using CountryService.DataAccess.ListQuery;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CountryService.WebApi.ListQuery;

public interface IQueryValidator
{
    void Validate(
        ModelStateDictionary modelState,
        string[] filterableProperties,
        string[] SortableProperties,
        Func<string, DataType> getDataType,
        int offset,
        int limit,
        string[]? filters,
        string[]? sorts);
}
