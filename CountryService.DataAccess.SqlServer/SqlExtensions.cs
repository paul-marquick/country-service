using Microsoft.Data.SqlClient;

namespace CountryService.DataAccess.SqlServer;

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
