using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace CountryService.DataAccess;

public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string connectionString;

    public SqlServerConnectionFactory(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public DbConnection CreateDbConnection()
    {
        return new SqlConnection(connectionString);
    }
}
