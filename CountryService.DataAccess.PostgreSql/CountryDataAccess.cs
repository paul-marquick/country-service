using CountryService.DataAccess.Exceptions;
using CountryService.DataAccess.Models.Country;
using Npgsql;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using CountryService.DataAccess.ListQuery;

namespace CountryService.DataAccess.PostgreSql;

public class CountryDataAccess : ICountryDataAccess
{
    private const string selectColumns = "\"iso_2\", \"iso_3\", \"iso_number\", \"name\", \"calling_code\"";
    private readonly ILogger<CountryDataAccess> logger;

    public CountryDataAccess(ILogger<CountryDataAccess> logger)
    {
        this.logger = logger;
    }    

    
    public async Task<(int, List<Country>)> CountryQueryAsync(Query query, DbConnection dbConnection, DbTransaction dbTransaction)
    {
        logger.LogDebug("CountryQueryAsync");

        string filterSql = CreateQueryWhereClauseSql(query.Filters);

        // Query for total.
        string totalSql = $"SELECT COUNT(1) FROM \"country\" {filterSql};";

        await using NpgsqlCommand totalDbCommand = new NpgsqlCommand(totalSql, (NpgsqlConnection) dbConnection, (NpgsqlTransaction) dbTransaction);
        AddQueryWhereClauseParameters(totalDbCommand, query.Filters);
        await using NpgsqlDataReader totalDbDataReader = await totalDbCommand.ExecuteReaderAsync();

        totalDbDataReader.Read();
        int total = totalDbDataReader.GetInt32(0);
       
        // Query for a paged list.

        string querySql = $"SELECT {selectColumns} FROM \"country\" {filterSql} ";

        await using NpgsqlCommand queryDbCommand = new NpgsqlCommand(querySql, (NpgsqlConnection) dbConnection, (NpgsqlTransaction) dbTransaction);
        AddQueryWhereClauseParameters(queryDbCommand, query.Filters);
        await using NpgsqlDataReader queryDbDataReader = await queryDbCommand.ExecuteReaderAsync();

        List<Country> countries = new List<Country>();

        while (queryDbDataReader.Read())
        {
            countries.Add(ReadData(queryDbDataReader));
        }

        return (total, countries);
    }

    private static string CreateQueryWhereClauseSql(List<Filter>? filters)
    {
        if (filters == null)
        {
            return string.Empty;
        }
        else
        {
            string result = "WHERE ";

            for (int i = 0; filters.Count > i; i++)
            {
                result += $"\"{GetColumnName(filters[i].PropertyName)}\" {ComparisonOperatorConverter.GetComparisonOperatorSql(filters[i].ComparisonOperator)} @{filters[i].PropertyName} ";

                if (i < filters.Count)
                {
                    result += "AND ";
                }
            }

            return result;
        }
    }

    private static void AddQueryWhereClauseParameters(NpgsqlCommand dbCommand, List<Filter>? filters)
    {
        if (filters != null)
        {
            foreach (Filter filter in filters)
            {
                dbCommand.Parameters.AddWithValue(filter.PropertyName, filter.Value);
            }
        }
    }

    private static string GetColumnName(string propertyName)
    {
        switch (propertyName)
        {
            case "Iso2":
                return "iso_2";

            case "Iso3":
                return "iso_3";

            case "IsoNumber":
                return "iso_number";

            case "Name":
                return "name";

            case "CallingCode":
                return "calling_code";

            default:
                throw new ArgumentException($"Unknown country property name: {propertyName}");
        }
    }
    
    public async Task<List<Country>> SelectCountriesAsync(DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug("SelectCountriesAsync");

        string sql = $"SELECT {selectColumns} FROM \"country\" ORDER BY \"name\" ASC";

        await using NpgsqlCommand dbCommand = new NpgsqlCommand(sql, (NpgsqlConnection)dbConnection, (NpgsqlTransaction?)dbTransaction);
        await using NpgsqlDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();

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

        string sql = "SELECT iso_2, name FROM \"country\" ORDER BY \"name\" ASC";

        await using NpgsqlCommand dbCommand = new NpgsqlCommand(sql, (NpgsqlConnection)dbConnection, (NpgsqlTransaction?)dbTransaction);
        await using NpgsqlDataReader dbDataReader = await dbCommand.ExecuteReaderAsync();

        List<CountryLookup> countryLookups = new List<CountryLookup>();

        while (dbDataReader.Read())
        {
            countryLookups.Add(new CountryLookup(dbDataReader.GetString(0), dbDataReader.GetString(1)));
        }

        return countryLookups;
    }

    public async Task<Country> SelectCountryByIso2Async(string iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"SelectCountryByIso2Async, iso2: {iso2}");

        string sql = $"SELECT {selectColumns} FROM \"country\" WHERE \"iso_2\" = @Iso2";

        await using NpgsqlCommand dbCommand = new NpgsqlCommand(sql, (NpgsqlConnection)dbConnection, (NpgsqlTransaction?)dbTransaction);
        dbCommand.Parameters.AddWithValue("Iso2", iso2);

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

    public async Task<int> InsertCountryAsync(Country country, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"InsertCountryAsync, country: Iso2: {country.Iso2}, Iso3: {country.Iso3}, IsoNumber: {country.IsoNumber}, Name: {country.Name}, CallingCode: {country.CallingCode}.");

        try
        {
            string sql = "INSERT INTO \"country\" (\"iso_2\", \"iso_3\", \"iso_number\", \"name\", \"calling_code\") VALUES (@Iso2, @Iso3, @IsoNumber, @Name, @CallingCode)";

            await using NpgsqlCommand dbCommand = new NpgsqlCommand(sql, (NpgsqlConnection)dbConnection, (NpgsqlTransaction?)dbTransaction);
            dbCommand.Parameters.AddWithValue("Iso2", country.Iso2);
            dbCommand.Parameters.AddWithValue("Iso3", country.Iso3);
            dbCommand.Parameters.AddWithValue("IsoNumber", country.IsoNumber);
            dbCommand.Parameters.AddWithValue("Name", country.Name);
            dbCommand.Parameters.AddWithNullableValue("CallingCode", country.CallingCode);

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

    public async Task<int> UpdateCountryByIso2Async(string iso2, Country country, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"UpdateCountryByIso2Async, iso2: {iso2},  country: Iso2: {country.Iso2}, Iso3: {country.Iso3}, IsoNumber: {country.IsoNumber}, Name: {country.Name}, CallingCode: {country.CallingCode}.");

        try
        {
            string sql = "UPDATE \"country\" SET \"iso_2\" = @Iso2, \"iso_3\" = @Iso3, \"iso_number\" = @IsoNumber, \"name\" = @Name, \"calling_code\" = @CallingCode WHERE \"iso_2\" = @pIso2";

            await using NpgsqlCommand dbCommand = new NpgsqlCommand(sql, (NpgsqlConnection)dbConnection, (NpgsqlTransaction?)dbTransaction);
            dbCommand.Parameters.AddWithValue("Iso2", country.Iso2);
            dbCommand.Parameters.AddWithValue("Iso3", country.Iso3);
            dbCommand.Parameters.AddWithValue("IsoNumber", country.IsoNumber);
            dbCommand.Parameters.AddWithValue("Name", country.Name);
            dbCommand.Parameters.AddWithNullableValue("CallingCode", country.CallingCode);
            dbCommand.Parameters.AddWithValue("Iso_2", iso2);

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

    public async Task<int> PartialUpdateCountryByIso2Async(string iso2, Country country, List<string> dirtyColumns, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"PartialUpdateCountryByIso2Async, iso2: {iso2}, dirtyColumns: {dirtyColumns}, country: Iso2: {country.Iso2}, Iso3: {country.Iso3}, IsoNumber: {country.IsoNumber}, Name: {country.Name}, CallingCode: {country.CallingCode}.");

        try
        {
            string sql = "UPDATE \"country\" SET ";

            await using NpgsqlCommand dbCommand = new NpgsqlCommand();
            dbCommand.Connection = (NpgsqlConnection)dbConnection;
            dbCommand.Transaction = (NpgsqlTransaction?)dbTransaction;

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

    public async Task<int> DeleteCountryByIso2Async(string iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"DeleteCountryByIso2Async, iso2: {iso2}");

        string sql = "DELETE FROM \"country\" WHERE \"iso_2\" = @Iso2";

        await using NpgsqlCommand dbCommand = new NpgsqlCommand(sql, (NpgsqlConnection)dbConnection, (NpgsqlTransaction?)dbTransaction);
        dbCommand.Parameters.AddWithValue("Iso2", iso2);

        return await dbCommand.ExecuteNonQueryAsync();
    }

    public async Task<bool> DoesCountryNameExistAsync(string name, string? iso2, DbConnection dbConnection, DbTransaction? dbTransaction = null)
    {
        logger.LogDebug($"DoesCountryNameExistAsync, name: {name}, iso2: {iso2}");

        string sql = "SELECT 1 FROM \"country\" WHERE \"name\" = @Name";

        await using NpgsqlCommand dbCommand = new NpgsqlCommand(sql, (NpgsqlConnection)dbConnection, (NpgsqlTransaction?)dbTransaction);
        dbCommand.Parameters.AddWithValue("name", name);

        if (iso2 != null)
        {
            dbCommand.CommandText += " AND iso_2 <> @Iso2;";
            dbCommand.Parameters.AddWithValue("Iso2", iso2);
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
