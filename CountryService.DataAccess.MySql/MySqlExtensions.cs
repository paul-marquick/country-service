using MySql.Data.MySqlClient;

namespace CountryService.DataAccess.MySql;

internal static class MySqlExtensions
{
    public static void AddWithNullableValue(this MySqlParameterCollection parameters, string parameterName, object? parameterValue)
    {
        parameters.AddWithValue(parameterName, parameterValue == null ? DBNull.Value : parameterValue);
    }

    public static string? GetNullableString(this MySqlDataReader dbDataReader, int ordinal)
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
