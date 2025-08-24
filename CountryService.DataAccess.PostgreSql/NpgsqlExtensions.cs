using System;
using Npgsql;

namespace CountryService.DataAccess.PostgreSql;

internal static class NpgsqlExtensions
{
    public static void AddWithNullableValue(this NpgsqlParameterCollection parameters, string parameterName, object? parameterValue)
    {
        parameters.AddWithValue(parameterName, parameterValue == null ? DBNull.Value : parameterValue);
    }

    public static string? GetNullableString(this NpgsqlDataReader dbDataReader, int ordinal)
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
