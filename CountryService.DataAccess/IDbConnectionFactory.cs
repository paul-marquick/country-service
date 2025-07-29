using System.Data.Common;

namespace CountryService.DataAccess;

public interface IDbConnectionFactory
{
    DbConnection CreateDbConnection();
}
