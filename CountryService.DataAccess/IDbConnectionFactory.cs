using System.Data.Common;

namespace CountryService.DataAccess;

/// <summary>
/// Defines a factory for creating instances of <see cref="DbConnection"/>.
/// </summary>
/// <remarks>Implementations of this interface are responsible for providing a configured instance of  <see
/// cref="DbConnection"/>. This can be used to abstract the creation of database connections,  enabling dependency
/// injection and simplifying connection management.</remarks>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates and returns a new instance of a database connection.
    /// </summary>
    /// <remarks>The returned <see cref="DbConnection"/> instance is not opened.  Call <see
    /// cref="DbConnection.Open"/> to establish the connection to the database. Ensure proper disposal of the connection
    /// to release resources.</remarks>
    /// <returns>A new <see cref="DbConnection"/> instance configured for the database.</returns>
    DbConnection CreateDbConnection();
}
