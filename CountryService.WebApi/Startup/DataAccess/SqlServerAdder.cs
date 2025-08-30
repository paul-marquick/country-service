using CountryService.DataAccess;
using CountryService.DataAccess.ListQuery;
using CountryService.DataAccess.SqlServer.ListQuery;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CountryService.WebApi.Startup.DataAccess;

internal static class SqlServerAdder
{
    public static void AddSqlServer(this WebApplicationBuilder builder, string connectionString)
    {
        Console.WriteLine("Add SQL Server data access.");

        builder.Services.AddSingleton<IDbConnectionFactory>(new CountryService.DataAccess.SqlServer.DbConnectionFactory(connectionString));
        builder.Services.AddSingleton<ICountryDataAccess, CountryService.DataAccess.SqlServer.CountryDataAccess>();
        builder.Services.AddSingleton<ISqlCreator, SqlCreator>();
    }
}
