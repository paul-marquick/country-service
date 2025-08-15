using CountryService.DataAccess.Exceptions;
using CountryService.DataAccess.Models.Country;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace CountryService.DataAccess.MySql;

public class CountryDataAccess : ICountryDataAccess
{
    private const string selectColumns = "`iso_2`, `iso_3`, `iso_number`, `name`, `calling_code`";
    private readonly ILogger<CountryDataAccess> logger;

    public CountryDataAccess(ILogger<CountryDataAccess> logger)
    {
        this.logger = logger;
    }

    public async Task<List<Country>> SelectCountriesAsync(DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug("SelectListAsync");

        string sql = $"SELECT {selectColumns} FROM `country` ORDER BY `name` ASC";

        await using MySqlCommand dbCommand = new MySqlCommand(sql, (MySqlConnection)dbConnection, (MySqlTransaction?)dbTransaction);
        await using MySqlDataReader dbDataReader = dbCommand.ExecuteReader();

        List<Country> countryList = new List<Country>();

        while (dbDataReader.Read())
        {
            countryList.Add(ReadData(dbDataReader));
        }

        return countryList;
    }

    public async Task<List<CountryLookup>> SelectCountryLookupsAsync(DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug("SelectLookupListAsync");

        string sql = "SELECT `iso_2`, `name` FROM `country` ORDER BY `name` ASC";

        await using MySqlCommand dbCommand = new MySqlCommand(sql, (MySqlConnection)dbConnection, (MySqlTransaction?)dbTransaction);
        await using MySqlDataReader dbDataReader = dbCommand.ExecuteReader();

        List<CountryLookup> countryLookupList = new List<CountryLookup>();

        while (dbDataReader.Read())
        {
            countryLookupList.Add(new CountryLookup(dbDataReader.GetString(0), dbDataReader.GetString(1)));
        }

        return countryLookupList;
    }

    public async Task<Country> SelectCountryByIso2Async(string iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"SelectByIso2Async, iso2: {iso2}");

        string sql = $"SELECT {selectColumns} FROM `country` WHERE `iso_2` = @Iso2";

        await using MySqlCommand dbCommand = new MySqlCommand(sql, (MySqlConnection)dbConnection, (MySqlTransaction?)dbTransaction);
        dbCommand.Parameters.AddWithValue("Iso2", iso2);

        await using MySqlDataReader dbDataReader = dbCommand.ExecuteReader();

        if (dbDataReader.Read())
        {
            return ReadData(dbDataReader);
        }
        else
        {
            throw new CountryNotFoundException($"Country not found, Iso2 : {iso2}");
        }
    }

    public async Task<int> InsertCountryAsync(Country country, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"InsertAsync, country: Iso2: {country.Iso2}, Iso3: {country.Iso3}, IsoNumber: {country.IsoNumber}, Name: {country.Name}, CallingCode: {country.CallingCode}.");

        try
        {
            string sql = "INSERT INTO `country` (`iso_2`, `iso_3`, `iso_number`, `name`, `calling_code`) VALUES (@Iso2, @Iso3, @IsoNumber, @Name, @CallingCode)";

            await using MySqlCommand dbCommand = new MySqlCommand(sql, (MySqlConnection)dbConnection, (MySqlTransaction?)dbTransaction);
            dbCommand.Parameters.AddWithValue("Iso2", country.Iso2);
            dbCommand.Parameters.AddWithValue("Iso3", country.Iso3);
            dbCommand.Parameters.AddWithValue("IsoNumber", country.IsoNumber);
            dbCommand.Parameters.AddWithValue("Name", country.Name);
            dbCommand.Parameters.AddWithNullableValue("CallingCode", country.CallingCode);

            return await dbCommand.ExecuteNonQueryAsync();
        }
        catch (MySqlException mySqlException)
        {
            var constraintName = Utils.GetConstraintName(logger, mySqlException.Message);

            if (constraintName == null)
            {
                throw new DataAccessException("Constraint name is null.", mySqlException);
            }
            else
            {
                var dataExceptionType = Utils.GetDataExceptionType(logger, mySqlException.Message);

                if (dataExceptionType == DataExceptionType.Duplication)
                {
                    switch (constraintName)
                    {
                        case Constraints.PrimaryKeyCountryIso2:
                            throw new CountryIso2DuplicatedException($"Country Iso2 (PK) : {country.Iso2}", mySqlException);

                        case Constraints.UniqueIndexCountryIso3:
                            throw new CountryIso3DuplicatedException($"Country Iso3 : {country.Iso3}", mySqlException);

                        case Constraints.UniqueIndexCountryIsoNumber:
                            throw new CountryIsoNumberDuplicatedException($"Country IsoNumber : {country.IsoNumber}", mySqlException);

                        case Constraints.UniqueIndexCountryName:
                            throw new CountryNameDuplicatedException($"Country Name : {country.Name}", mySqlException);

                        default:
                            throw new DataAccessException($"Unknown constraint name : {constraintName}", mySqlException);
                    }
                }
                else
                {
                    throw new DataAccessException($"Unknown data exception type : {dataExceptionType}", mySqlException);
                }
            }
        }
    }

    public async Task<int> UpdateCountryByIso2Async(string iso2, Country country, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"UpdateByIso2Async, iso2: {iso2},  country: Iso2: {country.Iso2}, Iso3: {country.Iso3}, IsoNumber: {country.IsoNumber}, Name: {country.Name}, CallingCode: {country.CallingCode}.");

        try
        {
            string sql = "UPDATE `country` SET `iso_2` = @Iso2, `iso_3` = @Iso3, `iso_number` = @IsoNumber, `name` = @Name, `calling_code` = @CallingCode WHERE `iso_2` = @pIso2";

            await using MySqlCommand dbCommand = new MySqlCommand(sql, (MySqlConnection)dbConnection, (MySqlTransaction?)dbTransaction);
            dbCommand.Parameters.AddWithValue("Iso_2", country.Iso2);
            dbCommand.Parameters.AddWithValue("Iso3", country.Iso3);
            dbCommand.Parameters.AddWithValue("IsoNumber", country.IsoNumber);
            dbCommand.Parameters.AddWithValue("Name", country.Name);
            dbCommand.Parameters.AddWithNullableValue("CallingCode", country.CallingCode);
            dbCommand.Parameters.AddWithValue("pIso2", iso2);

            return await dbCommand.ExecuteNonQueryAsync();
        }
        catch (MySqlException mySqlException)
        {
            var constraintName = Utils.GetConstraintName(logger, mySqlException.Message);

            if (constraintName == null)
            {
                throw new DataAccessException("Constraint name is null.", mySqlException);
            }
            else
            {
                var dataExceptionType = Utils.GetDataExceptionType(logger, mySqlException.Message);

                if (dataExceptionType == DataExceptionType.Duplication)
                {
                    switch (constraintName)
                    {
                        case Constraints.PrimaryKeyCountryIso2:
                            throw new CountryIso2DuplicatedException($"Country Iso2 (PK) : {country.Iso2}", mySqlException);

                        case Constraints.UniqueIndexCountryIso3:
                            throw new CountryIso3DuplicatedException($"Country Iso3 : {country.Iso3}", mySqlException);

                        case Constraints.UniqueIndexCountryIsoNumber:
                            throw new CountryIsoNumberDuplicatedException($"Country IsoNumber : {country.IsoNumber}", mySqlException);

                        case Constraints.UniqueIndexCountryName:
                            throw new CountryNameDuplicatedException($"Country Name : {country.Name}", mySqlException);

                        default:
                            throw new DataAccessException($"Unknown constraint name : {constraintName}", mySqlException);
                    }
                }
                else
                {
                    throw new DataAccessException($"Unknown data exception type : {dataExceptionType}", mySqlException);
                }
            }
        }
    }

    public async Task<int> PartialUpdateCountryByIso2Async(string iso2, Country country, List<string> dirtyColumns, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"PartialUpdateByIso2Async, iso2: {iso2}, dirtyColumns: {dirtyColumns}, country: Iso2: {country.Iso2}, Iso3: {country.Iso3}, IsoNumber: {country.IsoNumber}, Name: {country.Name}, CallingCode: {country.CallingCode}.");

        try
        {
            string sql = "UPDATE `country` SET ";

            await using MySqlCommand dbCommand = new MySqlCommand(sql, (MySqlConnection)dbConnection, (MySqlTransaction?)dbTransaction);
            dbCommand.Connection = (MySqlConnection)dbConnection;
            dbCommand.Transaction = (MySqlTransaction?)dbTransaction;

            if (dirtyColumns.Contains("Iso2", StringComparer.OrdinalIgnoreCase))
            {
                sql += "\"iso_2\" = @Iso2,";
                dbCommand.Parameters.AddWithValue("Iso2", country.Iso2);
            }

            if (dirtyColumns.Contains("Iso3", StringComparer.OrdinalIgnoreCase))
            {
                sql += " \"iso_3\" = @Iso3,";
                dbCommand.Parameters.AddWithValue("Iso3", country.Iso3);
            }

            if (dirtyColumns.Contains("IsoNumber", StringComparer.OrdinalIgnoreCase))
            {
                sql += " \"iso_number\" = @IsoNumber,";
                dbCommand.Parameters.AddWithValue("IsoNumber", country.IsoNumber);
            }

            if (dirtyColumns.Contains("Name", StringComparer.OrdinalIgnoreCase))
            {
                sql += " \"name\" = @Name,";
                dbCommand.Parameters.AddWithValue("Name", country.Name);
            }

            if (dirtyColumns.Contains("CallingCode", StringComparer.OrdinalIgnoreCase))
            {
                sql += " \"calling_code\" = @CallingCode";
                dbCommand.Parameters.AddWithNullableValue("CallingCode", country.CallingCode);
            }

            // Remove the last comma if any columns were updated.
            sql = sql.TrimEnd(',');

            sql += " WHERE \"iso_2\" = @pIso2";

            dbCommand.Parameters.AddWithValue("pIso2", iso2);

            dbCommand.CommandText = sql;

            return await dbCommand.ExecuteNonQueryAsync();
        }
        catch (MySqlException mySqlException)
        {
            var constraintName = Utils.GetConstraintName(logger, mySqlException.Message);

            if (constraintName == null)
            {
                throw new DataAccessException("Constraint name is null.", mySqlException);
            }
            else
            {
                var dataExceptionType = Utils.GetDataExceptionType(logger, mySqlException.Message);

                if (dataExceptionType == DataExceptionType.Duplication)
                {
                    switch (constraintName)
                    {
                        case Constraints.PrimaryKeyCountryIso2:
                            throw new CountryIso2DuplicatedException($"Country Iso2 (PK) : {country.Iso2}", mySqlException);

                        case Constraints.UniqueIndexCountryIso3:
                            throw new CountryIso3DuplicatedException($"Country Iso3 : {country.Iso3}", mySqlException);

                        case Constraints.UniqueIndexCountryIsoNumber:
                            throw new CountryIsoNumberDuplicatedException($"Country IsoNumber : {country.IsoNumber}", mySqlException);

                        case Constraints.UniqueIndexCountryName:
                            throw new CountryNameDuplicatedException($"Country Name : {country.Name}", mySqlException);

                        default:
                            throw new DataAccessException($"Unknown constraint name : {constraintName}", mySqlException);
                    }
                }
                else
                {
                    throw new DataAccessException($"Unknown data exception type : {dataExceptionType}", mySqlException);
                }
            }
        }
    }

    public async Task<int> DeleteCountryByIso2Async(string iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"DeleteByIso2Async, iso2: {iso2}");

        string sql = "DELETE FROM `country` WHERE `iso_2` = @Iso2";

        await using MySqlCommand dbCommand = new MySqlCommand(sql, (MySqlConnection)dbConnection, (MySqlTransaction?)dbTransaction);
        dbCommand.Parameters.AddWithValue("Iso2", iso2);

        return await dbCommand.ExecuteNonQueryAsync();
    }

    public async Task<bool> DoesCountryNameExistAsync(string name, string? iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"DoesCountryNameExistAsync, name: {name}, iso2: {iso2}");

        string sql = "SELECT 1 FROM `country` WHERE `name` = @Name";

        await using MySqlCommand dbCommand = new MySqlCommand(sql, (MySqlConnection)dbConnection, (MySqlTransaction?)dbTransaction);
        dbCommand.Parameters.AddWithValue("Name", name);

        if (iso2 != null)
        {
            dbCommand.CommandText += " AND `iso_2` <> @Iso2;";
            dbCommand.Parameters.AddWithValue("Iso2", iso2);
        }

        await using MySqlDataReader dbDataReader = dbCommand.ExecuteReader();

        return dbDataReader.Read();
    }

    private static class Constraints
    {
        internal const string PrimaryKeyCountryIso2 = "country.PRIMARY";
        internal const string UniqueIndexCountryIso3 = "country.iso_3_UNIQUE";
        internal const string UniqueIndexCountryIsoNumber = "country.iso_number_UNIQUE";
        internal const string UniqueIndexCountryName = "country.name_UNIQUE";
    }

    private static Country ReadData(MySqlDataReader dbDataReader)
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