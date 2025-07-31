using Microsoft.Extensions.Logging;
using Moq;

namespace CountryService.DataAccess.Test;

public class DatabaseFixture
{
    public DatabaseFixture()
    {
        // Sql Server.
        SqlServerCountryDataAccess = new SqlServer.CountryDataAccess(Mock.Of<ILogger<SqlServer.CountryDataAccess>>());

        // PostgreSql.
        PostgreSqlCountryDataAccess = new PostgreSql.CountryDataAccess(Mock.Of<ILogger<PostgreSql.CountryDataAccess>>());

        // MySql.
        MySqlCountryDataAccess = new MySql.CountryDataAccess(Mock.Of<ILogger<MySql.CountryDataAccess>>());
    }

    // Sql Server.
    public IDbConnectionFactory SqlServerDbConnectionFactory => new SqlServer.DbConnectionFactory(
            "Server=.; Database=CountryService; Trusted_Connection=true; TrustServerCertificate=true; MultipleActiveResultSets=true");
    public ICountryDataAccess SqlServerCountryDataAccess { get; private set; }

    // PosgreSql.
    public IDbConnectionFactory PostgreSqlDbConnectionFactory => new PostgreSql.DbConnectionFactory(
            "Server=localhost; Port=5433; Database=country_service; Password=password; User Id=postgres; Include Error Detail=true;");
    public ICountryDataAccess PostgreSqlCountryDataAccess { get; private set; }

    // MySql.
    public IDbConnectionFactory MySqlDbConnectionFactory => new MySql.DbConnectionFactory(
            "Server=localhost; Port=3306; Database=country_service; Uid=root; Pwd=Password1!;");
    public ICountryDataAccess MySqlCountryDataAccess { get; private set; }

    public static string CreateRandomString(int length)
    {
        Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static int CreateRandomInt32()
    {
        return new Random().Next(int.MaxValue);
    }
}
