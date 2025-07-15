using Microsoft.Data.SqlClient;

namespace CountryService.DataAccess;

public interface ICountryDataAccess
{
    Task<List<Country>> SelectAsync(SqlConnection sqlConnection, SqlTransaction? sqlTransaction = null);

    /// <summary>
    /// Selects a country row by iso2.
    /// </summary>
    /// <param name="iso2"></param>
    /// <param name="sqlConnection"></param>
    /// <param name="sqlTransaction"></param>
    /// <exception cref="DataAccessException"></exception>
    /// <returns>Country</returns>
    Task<Country> SelectByIso2Async(string iso2, SqlConnection sqlConnection, SqlTransaction? sqlTransaction = null);

    Task<int> InsertAsync(Country country, SqlConnection sqlConnection, SqlTransaction? sqlTransaction = null);

    Task<int> UpdateByIso2Async(string iso2, Country country, SqlConnection sqlConnection, SqlTransaction? sqlTransaction = null);

    Task<int> DeleteByIso2Async(string iso2, SqlConnection sqlConnection, SqlTransaction? sqlTransaction = null);
}
