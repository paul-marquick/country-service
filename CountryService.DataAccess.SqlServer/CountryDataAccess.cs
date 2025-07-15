using CountryService.DataAccess.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace CountryService.DataAccess.SqlServer;

public class CountryDataAccess : ICountryDataAccess
{
    private readonly ILogger<CountryDataAccess> logger;

    public CountryDataAccess(ILogger<CountryDataAccess> logger)
    {
        this.logger = logger;
    }

    public async Task<List<Country>> SelectAsync(SqlConnection sqlConnection, SqlTransaction? sqlTransaction = null)
    {
        string sql = "SELECT Iso2, Iso3, IsoNumber, Name FROM \"Country\" ORDER BY Name ASC;";

        using SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection, sqlTransaction);
        using SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();

        List<Country> countries = new List<Country>();

        while (sqlDataReader.Read())
        {
            countries.Add(new Country
            {
                Iso2 = sqlDataReader.GetString(0),
                Iso3 = sqlDataReader.GetString(1),
                IsoNumber = sqlDataReader.GetInt32(2),
                Name = sqlDataReader.GetString(3)
            });            
        }

        return countries;
    }

    public async Task<Country> SelectByIso2Async(string iso2, SqlConnection sqlConnection, SqlTransaction? sqlTransaction = null)
    {
        string sql = "SELECT Iso2, Iso3, IsoNumber, Name FROM \"Country\" WHERE Iso2 = @Iso2;";

        using SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection, sqlTransaction);
        sqlCommand.Parameters.AddWithValue("Iso2", iso2);

        using SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();

        if (sqlDataReader.Read())
        {
            return new Country
            {
                Iso2 = sqlDataReader.GetString(0),
                Iso3 = sqlDataReader.GetString(1),
                IsoNumber = sqlDataReader.GetInt32(2),
                Name = sqlDataReader.GetString(3)
            };
        }
        else
        {
            throw new CountryNotFoundException($"Country not found, Iso2 : {iso2}");
        }
    }

    public async Task<int> InsertAsync(Country country, SqlConnection sqlConnection, SqlTransaction? sqlTransaction = null)
    {
        try
        {
            string sql = "INSERT \"Country\" (\"Iso2\", \"Iso3\", \"IsoNumber\", \"Name\") VALUES (@Iso2, @Iso3, @IsoNumber, @Name)";

            using SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection, sqlTransaction);
            sqlCommand.Parameters.AddWithValue("Iso2", country.Iso2);
            sqlCommand.Parameters.AddWithValue("Iso3", country.Iso3);
            sqlCommand.Parameters.AddWithValue("IsoNumber", country.IsoNumber);
            sqlCommand.Parameters.AddWithValue("Name", country.Name);

            return await sqlCommand.ExecuteNonQueryAsync();
        }
        catch (SqlException sqlException)
        {
            var constraintName = Utils.GetConstraintName(sqlException.Message);

            if (constraintName == null)
            {
                throw new DataAccessException("Constraint name is null.", sqlException);
            }
            else
            {
                switch (constraintName)
                {
                    case Constraints.PrimaryKeyCountryIso2:
                        throw new CountryIso2DuplicatedException($"Country Iso2 (PK) : {country.Iso2}", sqlException);

                    case Constraints.UniqueIndexCountryIso3:
                        throw new CountryIso3DuplicatedException($"Country Iso3 : {country.Iso3}", sqlException);

                    case Constraints.UniqueIndexCountryIsoNumber:
                        throw new CountryIsoNumberDuplicatedException($"Country IsoNumber : {country.IsoNumber}", sqlException);

                    case Constraints.UniqueIndexCountryName:
                        throw new CountryNameDuplicatedException($"Country Name : {country.Name}", sqlException);

                    default:
                        throw new DataAccessException($"Unknown constraint name : {constraintName}", sqlException);
                }
            }
        }
    }

    public async Task<int> UpdateByIso2Async(string iso2, Country country, SqlConnection sqlConnection, SqlTransaction? sqlTransaction = null)
    {
        string sql = "UPDATE \"Country\" SET \"Iso2\" = @Iso2, \"Iso3\" = @Iso3, \"IsoNumber\" = @IsoNumber, \"Name\" = @Name WHERE \"Iso2\" = @pIso2";

        using SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection, sqlTransaction);
        sqlCommand.Parameters.AddWithValue("Iso2", country.Iso2);
        sqlCommand.Parameters.AddWithValue("Iso3", country.Iso3);
        sqlCommand.Parameters.AddWithValue("IsoNumber", country.IsoNumber);
        sqlCommand.Parameters.AddWithValue("Name", country.Name);
        sqlCommand.Parameters.AddWithValue("pIso2", iso2);

        return await sqlCommand.ExecuteNonQueryAsync();
    }

    public async Task<int> DeleteByIso2Async(string iso2, SqlConnection sqlConnection, SqlTransaction? sqlTransaction = null)
    {
        string sql = "DELETE FROM \"Country\" WHERE \"Iso2\" = @Iso2";

        using SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection, sqlTransaction);
        sqlCommand.Parameters.AddWithValue("Iso2", iso2);

        return await sqlCommand.ExecuteNonQueryAsync();
    }

    private static class Constraints
    {
        internal const string PrimaryKeyCountryIso2 = "PK_Country_Iso2";
        internal const string UniqueIndexCountryIso3 = "UI_Country_Iso3";
        internal const string UniqueIndexCountryIsoNumber = "UI_Country_IsoNumber";
        internal const string UniqueIndexCountryName = "UI_Country_Name";
    }
}
