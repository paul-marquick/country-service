using MySql.Data.MySqlClient;
using System.Data.Common;

namespace CountryService.DataAccess.MySql;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string connectionString;

    public DbConnectionFactory(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public DbConnection CreateDbConnection()
    {
        return new MySqlConnection(connectionString);
    }
}
