using CountryService.DataAccess.Exceptions;
using CountryService.DataAccess.ListQuery;
using CountryService.Models.Country;
using CountryService.DataAccess.SqlServer.ListQuery;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CountryService.DataAccess.SqlServer;

public class CountryDataAccess(ILogger<CountryDataAccess> logger, SqlCreator sqlCreator) : ICountryDataAccess
{
    private const string selectColumns = "\"Iso2\", \"Iso3\", \"IsoNumber\", \"Name\", \"CallingCode\"";
    
    public async Task<(int, List<Country>)> CountryQueryAsync(Query query, DbConnection dbConnection, DbTransaction dbTransaction)
    {
        logger.LogDebug("CountryQueryAsync");

        string filterSql = sqlCreator.CreateQueryWhereClauseSql(query.Filters);

        logger.LogDebug($"filterSql: {filterSql}");

        // Query for total.
        string totalSql = $"SELECT COUNT(1) FROM \"Country\" {filterSql};";
                
        logger.LogDebug($"totalSql: {totalSql}");

        await using SqlCommand totalDbCommand = new SqlCommand(totalSql, (SqlConnection)dbConnection, (SqlTransaction)dbTransaction);
        sqlCreator.AddQueryWhereClauseParameters(totalDbCommand, query.Filters);
        await using SqlDataReader totalDbDataReader = await totalDbCommand.ExecuteReaderAsync();

        totalDbDataReader.Read();
        int total = totalDbDataReader.GetInt32(0);

        // Query for a paged list.
        string orderByClause = sqlCreator.CreateQueryOrderByClauseSql(query.Sorts);

        string querySql = $"SELECT {selectColumns} FROM \"Country\" {filterSql} {orderByClause} OFFSET {query.OffSet} ROWS FETCH NEXT {query.Limit} ROWS ONLY;";

        logger.LogDebug($"querySql: {querySql}");
        
        await using SqlCommand queryDbCommand = new SqlCommand(querySql, (SqlConnection)dbConnection, (SqlTransaction)dbTransaction);
        sqlCreator.AddQueryWhereClauseParameters(queryDbCommand, query.Filters);
        await using SqlDataReader queryDbDataReader = await queryDbCommand.ExecuteReaderAsync();

        List<Country> countries = new List<Country>();

        while (queryDbDataReader.Read())
        {
            countries.Add(ReadData(queryDbDataReader));
        }

        return (total, countries);
    }

    public async Task<List<Country>> SelectCountriesAsync(DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug("SelectCountriesAsync");

        string sql = $"SELECT {selectColumns} FROM \"Country\" ORDER BY \"Name\" ASC";

        await using SqlCommand dbCommand = new SqlCommand(sql, (SqlConnection)dbConnection, (SqlTransaction?)dbTransaction);
        await using SqlDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();

        List<Country> countries = new List<Country>();

        while (dbDataReader.Read())
        {
            countries.Add(ReadData(dbDataReader));
        }

        return countries;
    }

    public async Task<List<CountryLookup>> SelectCountryLookupsAsync(DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug("SelectCountryLookupsAsync");

        string sql = "SELECT \"Iso2\", \"Name\" FROM \"Country\" ORDER BY \"Name\" ASC";

        await using SqlCommand dbCommand = new SqlCommand(sql, (SqlConnection)dbConnection, (SqlTransaction?)dbTransaction);
        await using SqlDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();

        List<CountryLookup> countryLookups = new List<CountryLookup>();

        while (dbDataReader.Read())
        {
            countryLookups.Add(
                new CountryLookup()
                {
                    Iso2 = dbDataReader.GetString(0),
                    Name = dbDataReader.GetString(1)
                }
            );
        }

        return countryLookups;
    }

    public async Task<Country> SelectCountryByIso2Async(string iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"SelectCountryByIso2Async, iso2: {iso2}");

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

    public async Task<int> InsertCountryAsync(Country country, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"InsertCountryAsync, country: Iso2: {country.Iso2}, Iso3: {country.Iso3}, IsoNumber: {country.IsoNumber}, Name: {country.Name}, CallingCode: {country.CallingCode}.");

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

    public async Task<int> UpdateCountryByIso2Async(string iso2, Country country, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"UpdateCountryByIso2Async, iso2: {iso2},  country: Iso2: {country.Iso2}, Iso3: {country.Iso3}, IsoNumber: {country.IsoNumber}, Name: {country.Name}, CallingCode: {country.CallingCode}.");

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

    public async Task<int> PartialUpdateCountryByIso2Async(string iso2, Country country, List<string> dirtyColumns, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"PartialUpdateCountryByIso2Async, iso2: {iso2}, dirtyColumns: {dirtyColumns}, country: Iso2: {country.Iso2}, Iso3: {country.Iso3}, IsoNumber: {country.IsoNumber}, Name: {country.Name}, CallingCode: {country.CallingCode}.");

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

    public async Task<int> DeleteCountryByIso2Async(string iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"DeleteCountryByIso2Async, iso2: {iso2}");

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
