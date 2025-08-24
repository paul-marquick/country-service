using CountryService.DataAccess.Exceptions;
using CountryService.DataAccess.ListQuery;
using CountryService.Models.Country;
using System.Data.Common;

namespace CountryService.DataAccess;

public interface ICountryDataAccess
{
    /// <summary>
    /// Gets a paged list of countries.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="dbConnection"></param>
    /// <param name="dbTransaction"></param>
    /// <returns>Total in query and a list of Country.</returns>
    Task<(int, List<Country>)> CountryQueryAsync(Query query, DbConnection dbConnection, DbTransaction dbTransaction);

    /// <summary>
    /// Gets a basic list of countries.
    /// </summary>
    /// <param name="dbConnection"></param>
    /// <param name="dbTransaction"></param>
    /// <returns>List of Country</returns>
    Task<List<Country>> SelectCountriesAsync(DbConnection dbConnection, DbTransaction? dbTransaction = null);

    /// <summary>
    /// Gets a lookup list of countries with Iso2 and Name.
    /// </summary>
    /// <param name="dbConnection"></param>
    /// <param name="dbTransaction"></param>
    /// <returns>List of CountryLookup</returns>
    Task<List<CountryLookup>> SelectCountryLookupsAsync(DbConnection dbConnection, DbTransaction? dbTransaction = null);

    /// <summary>
    /// Selects a country row by iso2.
    /// </summary>
    /// <param name="iso2"></param>
    /// <param name="dbConnection"></param>
    /// <param name="dbTransaction"></param>
    /// <exception cref="CountryNotFoundException"></exception>
    /// <returns>Country</returns>
    Task<Country> SelectCountryByIso2Async(string iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null);

    /// <summary>
    /// Inserts a country row into the database.
    /// </summary>
    /// <param name="country"></param>
    /// <param name="dbConnection"></param>
    /// <param name="dbTransaction"></param>
    /// <exception cref="CountryIso2DuplicatedException"></exception>
    /// <exception cref="CountryIso3DuplicatedException"></exception>
    /// <exception cref="CountryIsoNumberDuplicatedException"></exception>
    /// <exception cref="CountryNameDuplicatedException"></exception>
    /// <returns>Number of affected rows.</returns>
    Task<int> InsertCountryAsync(Country country, DbConnection dbConnection, DbTransaction? dbTransaction = null);

    /// <summary>
    /// Updates a country row in the database.
    /// </summary>
    /// <param name="iso2"></param>
    /// <param name="country"></param>
    /// <param name="dbConnection"></param>
    /// <param name="dbTransaction"></param>
    /// <exception cref="CountryNotFoundException"></exception>
    /// <exception cref="CountryIso2DuplicatedException"></exception>
    /// <exception cref="CountryIso3DuplicatedException"></exception>
    /// <exception cref="CountryIsoNumberDuplicatedException"></exception>
    /// <exception cref="CountryNameDuplicatedException"></exception>
    /// <returns>Number of affected rows.</returns>
    Task<int> UpdateCountryByIso2Async(string iso2, Country country, DbConnection dbConnection, DbTransaction? dbTransaction = null);

    /// <summary>
    /// Partial update a country row into the database.
    /// </summary>
    /// <param name="iso2"></param>
    /// <param name="country"></param>
    /// <param name="dirtyColumns"></param>
    /// <param name="dbConnection"></param>
    /// <param name="dbTransaction"></param>
    /// <exception cref="CountryNotFoundException"></exception>
    /// <exception cref="CountryIso2DuplicatedException"></exception>
    /// <exception cref="CountryIso3DuplicatedException"></exception>
    /// <exception cref="CountryIsoNumberDuplicatedException"></exception>
    /// <exception cref="CountryNameDuplicatedException"></exception>
    /// <returns>Number of affected rows.</returns>
    Task<int> PartialUpdateCountryByIso2Async(string iso2, Country country, List<string> dirtyColumns, DbConnection dbConnection, DbTransaction? dbTransaction = null);

    /// <summary>
    /// Deletes a country from the database.
    /// </summary>
    /// <param name="iso2"></param>
    /// <param name="dbConnection"></param>
    /// <param name="dbTransaction"></param>
    /// <returns>Number of affected rows.</returns>
    Task<int> DeleteCountryByIso2Async(string iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null);

    /// <summary>
    /// Checks if the country name is already in the database.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="iso2"></param>
    /// <param name="dbConnection"></param>
    /// <param name="dbTransaction"></param>
    /// <returns>bool</returns>
    Task<bool> DoesCountryNameExistAsync(string name, string? iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null);
}
