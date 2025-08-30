using CountryService.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CountryService.WebApi.Startup.DataAccess;

internal static class MySqlAdder
{
    public static void AddMySql(this WebApplicationBuilder builder, string connectionString)
    {
        Console.WriteLine("Add MySql data access.");

        builder.Services.AddSingleton<IDbConnectionFactory>(new CountryService.DataAccess.MySql.DbConnectionFactory(connectionString));
        builder.Services.AddSingleton<ICountryDataAccess, CountryService.DataAccess.MySql.CountryDataAccess>();
    }
}
