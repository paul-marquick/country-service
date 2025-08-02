using CountryService.DataAccess.Exceptions;
using CountryService.DataAccess.Models.Country;
using Npgsql;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace CountryService.DataAccess.PostgreSql;

public class CountryDataAccess : ICountryDataAccess
{
    private const string selectColumns = "\"iso_2\", \"iso_3\", \"iso_number\", \"name\", \"calling_code\"";
    private readonly ILogger<CountryDataAccess> logger;

    public CountryDataAccess(ILogger<CountryDataAccess> logger)
    {
        this.logger = logger;
    }

    public async Task<List<Country>> SelectListAsync(DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug("SelectListAsync");

        string sql = $"SELECT {selectColumns} FROM \"country\" ORDER BY \"name\" ASC";

        await using NpgsqlCommand dbCommand = new NpgsqlCommand(sql, (NpgsqlConnection)dbConnection, (NpgsqlTransaction?)dbTransaction);
        await using NpgsqlDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();

        List<Country> countryList = new List<Country>();

        while (dbDataReader.Read())
        {
            countryList.Add(ReadData(dbDataReader));
        }

        return countryList;
    }

    public async Task<List<CountryLookup>> SelectLookupListAsync(DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug("SelectLookupListAsync");

        string sql = "SELECT iso_2, name FROM \"country\" ORDER BY \"name\" ASC";

        await using NpgsqlCommand dbCommand = new NpgsqlCommand(sql, (NpgsqlConnection)dbConnection, (NpgsqlTransaction?)dbTransaction);
        await using NpgsqlDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();

        List<CountryLookup> countryLookupList = new List<CountryLookup>();

        while (dbDataReader.Read())
        {
            countryLookupList.Add(new CountryLookup(dbDataReader.GetString(0), dbDataReader.GetString(1)));
        }

        return countryLookupList;
    }

    public async Task<Country> SelectByIso2Async(string iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"SelectByIso2Async, iso2: {iso2}");

        string sql = $"SELECT {selectColumns} FROM \"country\" WHERE \"iso_2\" = @iso_2";

        await using NpgsqlCommand dbCommand = new NpgsqlCommand(sql, (NpgsqlConnection)dbConnection, (NpgsqlTransaction?)dbTransaction);
        dbCommand.Parameters.AddWithValue("iso_2", iso2);

        await using NpgsqlDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();

        if (dbDataReader.Read())
        {
            return ReadData(dbDataReader);
        }
        else
        {
            throw new CountryNotFoundException($"Country not found, Iso2 : {iso2}");
        }
    }

    public async Task<int> InsertAsync(Country country, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"InsertAsync, country: Iso2: {country.Iso2}, Iso3: {country.Iso3}, IsoNumber: {country.IsoNumber}, Name: {country.Name}, CallingCode: {country.CallingCode}.");

        try
        {
            string sql = "INSERT INTO \"country\" (\"iso_2\", \"iso_3\", \"iso_number\", \"name\", \"calling_code\") VALUES (@iso_2, @iso_3, @iso_number, @name, @calling_code)";

            await using NpgsqlCommand dbCommand = new NpgsqlCommand(sql, (NpgsqlConnection)dbConnection, (NpgsqlTransaction?)dbTransaction);
            dbCommand.Parameters.AddWithValue("iso_2", country.Iso2);
            dbCommand.Parameters.AddWithValue("iso_3", country.Iso3);
            dbCommand.Parameters.AddWithValue("iso_number", country.IsoNumber);
            dbCommand.Parameters.AddWithValue("name", country.Name);
            dbCommand.Parameters.AddWithNullableValue("calling_code", country.CallingCode);

            return await dbCommand.ExecuteNonQueryAsync();
        }
        catch (PostgresException postgresException)
        {
            if (postgresException.ConstraintName == null)
            {
                throw new DataAccessException("Constraint name is null.", postgresException);
            }
            else
            {
                switch (postgresException.ConstraintName)
                {
                    case Constraints.PrimaryKeyCountryIso2:
                        throw new CountryIso2DuplicatedException($"Country Iso2 (PK) : {country.Iso2}", postgresException);

                    case Constraints.UniqueIndexCountryIso3:
                        throw new CountryIso3DuplicatedException($"Country Iso3 : {country.Iso3}", postgresException);

                    case Constraints.UniqueIndexCountryIsoNumber:
                        throw new CountryIsoNumberDuplicatedException($"Country IsoNumber : {country.IsoNumber}", postgresException);

                    case Constraints.UniqueIndexCountryName:
                        throw new CountryNameDuplicatedException($"Country Name : {country.Name}", postgresException);

                    default:
                        throw new DataAccessException($"Unknown constraint name : {postgresException.ConstraintName}", postgresException);
                }
            }
        }
    }

    public async Task<int> UpdateByIso2Async(string iso2, Country country, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"UpdateByIso2Async, iso2: {iso2},  country: Iso2: {country.Iso2}, Iso3: {country.Iso3}, IsoNumber: {country.IsoNumber}, Name: {country.Name}, CallingCode: {country.CallingCode}.");

        try
        {
            string sql = "UPDATE \"country\" SET \"iso_2\" = @iso_2, \"iso_3\" = @iso_3, \"iso_number\" = @iso_number, \"name\" = @name, \"calling_code\" = @calling_code WHERE \"iso_2\" = @p_iso_2";

            await using NpgsqlCommand dbCommand = new NpgsqlCommand(sql, (NpgsqlConnection)dbConnection, (NpgsqlTransaction?)dbTransaction);
            dbCommand.Parameters.AddWithValue("iso_2", country.Iso2);
            dbCommand.Parameters.AddWithValue("iso_3", country.Iso3);
            dbCommand.Parameters.AddWithValue("iso_number", country.IsoNumber);
            dbCommand.Parameters.AddWithValue("name", country.Name);
            dbCommand.Parameters.AddWithNullableValue("calling_code", country.CallingCode);
            dbCommand.Parameters.AddWithValue("p_iso_2", iso2);

            return await dbCommand.ExecuteNonQueryAsync();
        }
        catch (PostgresException postgresException)
        {
            if (postgresException.ConstraintName == null)
            {
                throw new DataAccessException("Constraint name is null.", postgresException);
            }
            else
            {
                switch (postgresException.ConstraintName)
                {
                    case Constraints.PrimaryKeyCountryIso2:
                        throw new CountryIso2DuplicatedException($"Country Iso2 (PK) : {country.Iso2}", postgresException);

                    case Constraints.UniqueIndexCountryIso3:
                        throw new CountryIso3DuplicatedException($"Country Iso3 : {country.Iso3}", postgresException);

                    case Constraints.UniqueIndexCountryIsoNumber:
                        throw new CountryIsoNumberDuplicatedException($"Country IsoNumber : {country.IsoNumber}", postgresException);

                    case Constraints.UniqueIndexCountryName:
                        throw new CountryNameDuplicatedException($"Country Name : {country.Name}", postgresException);

                    default:
                        throw new DataAccessException($"Unknown constraint name : {postgresException.ConstraintName}", postgresException);
                }
            }
        }
    }

    public async Task<int> DeleteByIso2Async(string iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"DeleteByIso2Async, iso2: {iso2}");

        string sql = "DELETE FROM \"country\" WHERE \"iso_2\" = @iso_2";

        await using NpgsqlCommand dbCommand = new NpgsqlCommand(sql, (NpgsqlConnection)dbConnection, (NpgsqlTransaction?)dbTransaction);
        dbCommand.Parameters.AddWithValue("iso_2", iso2);

        return await dbCommand.ExecuteNonQueryAsync();
    }

    public async Task<bool> DoesCountryNameExistAsync(string name, string? iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"DoesCountryNameExistAsync, name: {name}, iso2: {iso2}");

        string sql = "SELECT 1 FROM \"country\" WHERE \"name\" = @name";

        await using NpgsqlCommand dbCommand = new NpgsqlCommand(sql, (NpgsqlConnection)dbConnection, (NpgsqlTransaction?)dbTransaction);
        dbCommand.Parameters.AddWithValue("name", name);

        if (iso2 != null)
        {
            dbCommand.CommandText += " AND iso_2 <> @iso_2;";
            dbCommand.Parameters.AddWithValue("iso_2", iso2);
        }

        await using NpgsqlDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();

        return dbDataReader.Read();
    }

    private static class Constraints
    {
        internal const string PrimaryKeyCountryIso2 = "pk-country-iso_2";
        internal const string UniqueIndexCountryIso3 = "ui-country-iso_3";
        internal const string UniqueIndexCountryIsoNumber = "pk-country-iso_number";
        internal const string UniqueIndexCountryName = "ui-country-name";
    }

    private static Country ReadData(NpgsqlDataReader dbDataReader)
    {
        return new Country
        {
            Iso2 = dbDataReader.GetString(0),
            Iso3 = dbDataReader.GetString(1),
            IsoNumber = dbDataReader.GetInt32(2),
            Name = dbDataReader.GetString(3),
            CallingCode = dbDataReader.GetNullableString(4)
        };
    }
}
