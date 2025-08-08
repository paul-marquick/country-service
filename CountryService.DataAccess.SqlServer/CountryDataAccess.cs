using CountryService.DataAccess.Exceptions;
using CountryService.DataAccess.Models.Country;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace CountryService.DataAccess.SqlServer;

public class CountryDataAccess : ICountryDataAccess
{
    private const string selectColumns = "\"Iso2\", \"Iso3\", \"IsoNumber\", \"Name\", \"CallingCode\"";
    private readonly ILogger<CountryDataAccess> logger;

    public CountryDataAccess(ILogger<CountryDataAccess> logger)
    {
        this.logger = logger;
    }

    public async Task<List<Country>> SelectListAsync(DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug("SelectListAsync");

        string sql = $"SELECT {selectColumns} FROM \"Country\" ORDER BY \"Name\" ASC";

        await using SqlCommand dbCommand = new SqlCommand(sql, (SqlConnection) dbConnection, (SqlTransaction?) dbTransaction);
        await using SqlDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();

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

        string sql = "SELECT \"Iso2\", \"Name\" FROM \"Country\" ORDER BY \"Name\" ASC";

        await using SqlCommand dbCommand = new SqlCommand(sql, (SqlConnection)dbConnection, (SqlTransaction?)dbTransaction);
        await using SqlDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();

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

        string sql = $"SELECT {selectColumns} FROM \"Country\" WHERE \"Iso2\" = @Iso2";

        await using SqlCommand dbCommand = new SqlCommand(sql, (SqlConnection)dbConnection, (SqlTransaction?)dbTransaction);
        dbCommand.Parameters.AddWithValue("Iso2", iso2);

        await using SqlDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();

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
            string sql = "INSERT INTO \"Country\" (\"Iso2\", \"Iso3\", \"IsoNumber\", \"Name\", \"CallingCode\") VALUES (@Iso2, @Iso3, @IsoNumber, @Name, @CallingCode)";

            await using SqlCommand dbCommand = new SqlCommand(sql, (SqlConnection)dbConnection, (SqlTransaction?)dbTransaction);
            dbCommand.Parameters.AddWithValue("Iso2", country.Iso2);
            dbCommand.Parameters.AddWithValue("Iso3", country.Iso3);
            dbCommand.Parameters.AddWithValue("IsoNumber", country.IsoNumber);
            dbCommand.Parameters.AddWithValue("Name", country.Name);
            dbCommand.Parameters.AddWithNullableValue("CallingCode", country.CallingCode);

            return await dbCommand.ExecuteNonQueryAsync();
        }
        catch (SqlException dbException)
        {
            if (dbException.Number == 2601 || dbException.Number == 2627) // 2601 index dup, 2627 pk dup.
            {
                var constraintName = Utils.GetConstraintName(logger, dbException.Message);

                if (constraintName == null)
                {
                    throw new DataAccessException("Constraint name is null.", dbException);
                }
                else
                {
                    switch (constraintName)
                    {
                        case Constraints.PrimaryKeyCountryIso2:
                            throw new CountryIso2DuplicatedException($"Country Iso2 (PK) : {country.Iso2}", dbException);

                        case Constraints.UniqueIndexCountryIso3:
                            throw new CountryIso3DuplicatedException($"Country Iso3 : {country.Iso3}", dbException);

                        case Constraints.UniqueIndexCountryIsoNumber:
                            throw new CountryIsoNumberDuplicatedException($"Country IsoNumber : {country.IsoNumber}", dbException);

                        case Constraints.UniqueIndexCountryName:
                            throw new CountryNameDuplicatedException($"Country Name : {country.Name}", dbException);

                        default:
                            throw new DataAccessException($"Unknown constraint name : {constraintName}", dbException);
                    }
                }
            }
            else
            {
                throw new DataAccessException($"Sql Server unknown error number: {dbException.Number}.", dbException);
            }
        }
    }

    public async Task<int> UpdateByIso2Async(string iso2, Country country, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"UpdateByIso2Async, iso2: {iso2},  country: Iso2: {country.Iso2}, Iso3: {country.Iso3}, IsoNumber: {country.IsoNumber}, Name: {country.Name}, CallingCode: {country.CallingCode}.");

        try
        {
            string sql = "UPDATE \"Country\" SET \"Iso2\" = @Iso2, \"Iso3\" = @Iso3, \"IsoNumber\" = @IsoNumber, \"Name\" = @Name, \"CallingCode\" = @CallingCode WHERE \"Iso2\" = @pIso2";

            await using SqlCommand dbCommand = new SqlCommand(sql, (SqlConnection)dbConnection, (SqlTransaction?)dbTransaction);
            dbCommand.Parameters.AddWithValue("Iso2", country.Iso2);
            dbCommand.Parameters.AddWithValue("Iso3", country.Iso3);
            dbCommand.Parameters.AddWithValue("IsoNumber", country.IsoNumber);
            dbCommand.Parameters.AddWithValue("Name", country.Name);
            dbCommand.Parameters.AddWithNullableValue("CallingCode", country.CallingCode);
            dbCommand.Parameters.AddWithValue("pIso2", iso2);

            return await dbCommand.ExecuteNonQueryAsync();
        }
        catch (SqlException dbException)
        {
            if (dbException.Number == 2601 || dbException.Number == 2627) 
            {
                var constraintName = Utils.GetConstraintName(logger, dbException.Message);

                if (constraintName == null)
                {
                    throw new DataAccessException("Constraint name is null.", dbException);
                }
                else
                {
                    switch (constraintName)
                    {
                        case Constraints.PrimaryKeyCountryIso2:
                            throw new CountryIso2DuplicatedException($"Country Iso2 (PK) : {country.Iso2}", dbException);

                        case Constraints.UniqueIndexCountryIso3:
                            throw new CountryIso3DuplicatedException($"Country Iso3 : {country.Iso3}", dbException);

                        case Constraints.UniqueIndexCountryIsoNumber:
                            throw new CountryIsoNumberDuplicatedException($"Country IsoNumber : {country.IsoNumber}", dbException);

                        case Constraints.UniqueIndexCountryName:
                            throw new CountryNameDuplicatedException($"Country Name : {country.Name}", dbException);

                        default:
                            throw new DataAccessException($"Unknown constraint name : {constraintName}", dbException);
                    }
                }
            }
            else
            {
                throw new DataAccessException($"Sql Server unknown error number: {dbException.Number}.", dbException);
            }
        }
    }

    public async Task<int> PartialUpdateByIso2Async(string iso2, Country country, List<string> dirtyColumns, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"PartialUpdateByIso2Async, iso2: {iso2}, dirtyColumns: {dirtyColumns}, country: Iso2: {country.Iso2}, Iso3: {country.Iso3}, IsoNumber: {country.IsoNumber}, Name: {country.Name}, CallingCode: {country.CallingCode}.");

        try
        {
            string sql = "UPDATE \"Country\" SET ";

            await using SqlCommand dbCommand = new SqlCommand();
            dbCommand.Connection = (SqlConnection)dbConnection;
            dbCommand.Transaction = (SqlTransaction?)dbTransaction;
            
            if (dirtyColumns.Contains("Iso2", StringComparer.OrdinalIgnoreCase))
            {
                sql += "\"Iso2\" = @Iso2,";
                dbCommand.Parameters.AddWithValue("Iso2", country.Iso2);
            }

            if (dirtyColumns.Contains("Iso3", StringComparer.OrdinalIgnoreCase))
            {
                sql += " \"Iso3\" = @Iso3,";
                dbCommand.Parameters.AddWithValue("Iso3", country.Iso3);
            }

            if (dirtyColumns.Contains("IsoNumber", StringComparer.OrdinalIgnoreCase))
            {
                sql += " \"IsoNumber\" = @IsoNumber,";
                dbCommand.Parameters.AddWithValue("IsoNumber", country.IsoNumber);
            }

            if (dirtyColumns.Contains("Name", StringComparer.OrdinalIgnoreCase))
            {
                sql += " \"Name\" = @Name,";
                dbCommand.Parameters.AddWithValue("Name", country.Name);
            }

            if (dirtyColumns.Contains("CallingCode", StringComparer.OrdinalIgnoreCase))
            {
                sql += " \"CallingCode\" = @CallingCode";
                dbCommand.Parameters.AddWithNullableValue("CallingCode", country.CallingCode);
            }

            // Remove the last comma if any columns were updated.
            sql = sql.TrimEnd(',');

            sql += " WHERE \"Iso2\" = @pIso2";

            dbCommand.Parameters.AddWithValue("pIso2", iso2);

            dbCommand.CommandText = sql;

            return await dbCommand.ExecuteNonQueryAsync();
        }
        catch (SqlException dbException)
        {
            if (dbException.Number == 2601 || dbException.Number == 2627)
            {
                var constraintName = Utils.GetConstraintName(logger, dbException.Message);

                if (constraintName == null)
                {
                    throw new DataAccessException("Constraint name is null.", dbException);
                }
                else
                {
                    switch (constraintName)
                    {
                        case Constraints.PrimaryKeyCountryIso2:
                            throw new CountryIso2DuplicatedException($"Country Iso2 (PK) : {country.Iso2}", dbException);

                        case Constraints.UniqueIndexCountryIso3:
                            throw new CountryIso3DuplicatedException($"Country Iso3 : {country.Iso3}", dbException);

                        case Constraints.UniqueIndexCountryIsoNumber:
                            throw new CountryIsoNumberDuplicatedException($"Country IsoNumber : {country.IsoNumber}", dbException);

                        case Constraints.UniqueIndexCountryName:
                            throw new CountryNameDuplicatedException($"Country Name : {country.Name}", dbException);

                        default:
                            throw new DataAccessException($"Unknown constraint name : {constraintName}", dbException);
                    }
                }
            }
            else
            {
                throw new DataAccessException($"Sql Server unknown error number: {dbException.Number}.", dbException);
            }
        }
    }

    public async Task<int> DeleteByIso2Async(string iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"DeleteByIso2Async, iso2: {iso2}");

        string sql = "DELETE FROM \"Country\" WHERE \"Iso2\" = @Iso2";

        await using SqlCommand dbCommand = new SqlCommand(sql, (SqlConnection)dbConnection, (SqlTransaction?)dbTransaction);
        dbCommand.Parameters.AddWithValue("Iso2", iso2);

        return await dbCommand.ExecuteNonQueryAsync();
    }

    public async Task<bool> DoesCountryNameExistAsync(string name, string? iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"DoesCountryNameExistAsync, name: {name}, iso2: {iso2}");

        string sql = "SELECT 1 FROM \"Country\" WHERE \"Name\" = @Name";

        await using SqlCommand dbCommand = new SqlCommand(sql, (SqlConnection)dbConnection, (SqlTransaction?)dbTransaction);
        dbCommand.Parameters.AddWithValue("Name", name);

        if (iso2 != null)
        {
            dbCommand.CommandText += " AND Iso2 <> @Iso2;";
            dbCommand.Parameters.AddWithValue("Iso2", iso2);
        }

        await using SqlDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();

        return dbDataReader.Read();
    }

    private static class Constraints
    {
        internal const string PrimaryKeyCountryIso2 = "PK_Country_Iso2";
        internal const string UniqueIndexCountryIso3 = "UI_Country_Iso3";
        internal const string UniqueIndexCountryIsoNumber = "UI_Country_IsoNumber";
        internal const string UniqueIndexCountryName = "UI_Country_Name";
    }

    private static Country ReadData(SqlDataReader dbDataReader)
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
