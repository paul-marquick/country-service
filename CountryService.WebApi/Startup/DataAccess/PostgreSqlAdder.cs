using CountryService.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CountryService.WebApi.Startup.DataAccess;

internal static class PostgreSqlAdder
{
    public static void AddPostgreSql(this WebApplicationBuilder builder, string connectionString)
    {
        Console.WriteLine("Add PostgreSql data access.");

        builder.Services.AddSingleton<IDbConnectionFactory>(new CountryService.DataAccess.PostgreSql.DbConnectionFactory(connectionString));
        builder.Services.AddSingleton<ICountryDataAccess, CountryService.DataAccess.PostgreSql.CountryDataAccess>();
    }
}
