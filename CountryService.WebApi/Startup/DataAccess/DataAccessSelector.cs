using CountryService.WebApi.Configuration;
using CountryService.WebApi.Startup.DataAccess;
using Microsoft.AspNetCore.Builder;
using System;

namespace CountryService.WebApi.Startup;

/// <summary>
/// Can simply switch between different database systems by editing the appsettings.json file.
/// Need to change the database system value and the database connection string.
/// </summary>
internal static class DataAccessSelector
{
    public static void AddDataAccess(this WebApplicationBuilder builder, string databaseSystem, string connectionString)
    {
        Console.WriteLine($"{databaseSystem} data access selected.");
        
        switch (databaseSystem)
        {
            case DatabaseSystem.SqlServer:
                builder.AddSqlServer(connectionString);
                break;

            case DatabaseSystem.PostgreSql:
                builder.AddPostgreSql(connectionString);
                break;

            case DatabaseSystem.MySql:
                builder.AddMySql(connectionString);
                break;

            default:
                throw new ArgumentOutOfRangeException($"Unknown database system, selected, '{databaseSystem}'.");
        }
    }
}
