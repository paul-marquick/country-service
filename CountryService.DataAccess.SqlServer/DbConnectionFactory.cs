using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace CountryService.DataAccess.SqlServer;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string connectionString;

    public DbConnectionFactory(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public DbConnection CreateDbConnection()
    {
        return new SqlConnection(connectionString);
    }
}
