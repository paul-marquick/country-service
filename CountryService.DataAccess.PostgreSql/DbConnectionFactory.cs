using Npgsql;
using System.Data.Common;

namespace CountryService.DataAccess.PostgreSql;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string connectionString;

    public DbConnectionFactory(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public DbConnection CreateDbConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
}
