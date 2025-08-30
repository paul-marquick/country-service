using CountryService.DataAccess.ListQuery;
using CountryService.Mappers.Country;
using CountryService.WebApi.ListQuery;
using CountryService.WebApi.Problems;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CountryService.WebApi.Startup;

internal static class CodeAdder
{
    public static void AddCode(this WebApplicationBuilder builder)
    {
        Console.WriteLine("Adding code dependencies.");

        builder.Services.AddSingleton<IProblemDetailsCreator, ProblemDetailsCreator>();
        builder.Services.AddSingleton<IQueryValidator, QueryValidator>();
        builder.Services.AddSingleton<IQueryReader, QueryReader>();
        builder.Services.AddSingleton<IComparisonOperatorConverter, ComparisonOperatorConverter>();
        builder.Services.AddSingleton<IComparisonOperatorDbType, ComparisonOperatorDbType>();
        builder.Services.AddSingleton<ISortDirectionConverter, SortDirectionConverter>();
        builder.Services.AddSingleton<ICountryMapper, CountryMapper>();
        builder.Services.AddSingleton<ICountryLookupMapper, CountryLookupMapper>();
    }
}
