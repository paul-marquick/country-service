using System;
using CountryService.DataAccess.ListQuery;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CountryService.WebApi.ListQuery;

/// <summary>
/// Defines a contract for validating query parameters, including filters, sorts, and pagination settings.
/// </summary>
/// <remarks>This interface is designed to validate query parameters commonly used in data retrieval operations. 
/// It ensures that the provided filters, sorting options, and pagination settings conform to the expected  rules and
/// constraints.</remarks>
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
