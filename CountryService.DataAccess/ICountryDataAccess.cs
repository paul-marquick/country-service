using CountryService.DataAccess.Exceptions;
using CountryService.DataAccess.Models;
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
    /// <exception cref="CountryNotFoundException"></exception>
    /// <returns>Country</returns>
    Task<Country> SelectByIso2Async(string iso2, SqlConnection sqlConnection, SqlTransaction? sqlTransaction = null);

    /// <summary>
    /// Inserts a country row into the database.
    /// </summary>
    /// <param name="country"></param>
    /// <param name="sqlConnection"></param>
    /// <param name="sqlTransaction"></param>
    /// <exception cref="CountryIso2DuplicatedException"></exception>
    /// <exception cref="CountryIso3DuplicatedException"></exception>
    /// <exception cref="CountryIsoNumberDuplicatedException"></exception>
    /// <exception cref="CountryNameDuplicatedException"></exception>
    /// <returns>Number of affected rows.</returns>
    Task<int> InsertAsync(Country country, SqlConnection sqlConnection, SqlTransaction? sqlTransaction = null);

    /// <summary>
    /// Inserts a country row into the database.
    /// </summary>
    /// <param name="iso2"></param>
    /// <param name="country"></param>
    /// <param name="sqlConnection"></param>
    /// <param name="sqlTransaction"></param>
    /// <exception cref="CountryNotFoundException"></exception>
    /// <exception cref="CountryIso2DuplicatedException"></exception>
    /// <exception cref="CountryIso3DuplicatedException"></exception>
    /// <exception cref="CountryIsoNumberDuplicatedException"></exception>
    /// <exception cref="CountryNameDuplicatedException"></exception>
    /// <returns>Number of affected rows.</returns>
    Task<int> UpdateByIso2Async(string iso2, Country country, SqlConnection sqlConnection, SqlTransaction? sqlTransaction = null);

    Task<int> DeleteByIso2Async(string iso2, SqlConnection sqlConnection, SqlTransaction? sqlTransaction = null);
}
