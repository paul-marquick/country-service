namespace CountryService.DataAccess.ListQuery;

/// <summary>
/// Defines a method for converting a sort direction into its corresponding SQL representation.
/// </summary>
/// <remarks>This interface is typically used to standardize the conversion of sort direction values (e.g., "ASC"
/// or "DESC")  into SQL-compatible strings. Implementations may define specific rules for handling invalid or custom
/// sort directions.</remarks>
public interface ISortDirectionConverter
{
    string GetSortDirectionSql(string sortDirection);
}
