using System;
using Microsoft.Data.SqlClient;

namespace CountryService.DataAccess.SqlServer;

/// <summary>
/// Provides extension methods for working with SQL-related objects, such as  <see cref="SqlParameterCollection"/> and
/// <see cref="SqlDataReader"/>.
/// </summary>
/// <remarks>This class contains utility methods to simplify common operations when interacting  with SQL Server,
/// such as handling nullable values in parameters and reading nullable  strings from a data reader.</remarks>
internal static class SqlExtensions
{
    public static void AddWithNullableValue(this SqlParameterCollection parameters, string parameterName, object? parameterValue)
    {
        parameters.AddWithValue(parameterName, parameterValue == null ? DBNull.Value : parameterValue);
    }

    public static string? GetNullableString(this SqlDataReader dbDataReader, int ordinal)
    {
        if (dbDataReader.IsDBNull(ordinal))
        {
            return null;
        }
        else
        {
           return dbDataReader.GetString(ordinal);
        }
    }
}
