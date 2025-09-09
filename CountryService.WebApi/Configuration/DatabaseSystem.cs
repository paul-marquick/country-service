namespace CountryService.WebApi.Configuration;

/// <summary>
/// Provides constants representing supported database systems.
/// </summary>
/// <remarks>This class defines string constants for commonly used database systems, such as PostgreSQL, MySQL,
/// and SQL Server. These constants can be used to identify or configure database types in applications.</remarks>
public static class DatabaseSystem
{
    public const string PostgreSql = "PostgreSql";
    public const string MySql = "MySql";
    public const string SqlServer = "SqlServer";
}
