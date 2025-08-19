namespace CountryService.DataAccess.ListQuery;

public static class SortDirectionConverter
{
    public static string GetSortDirectionSql(string sortDirection)
    {
        switch (sortDirection)
        {
            case SortDirection.Ascending:
                return "ASC";

            case SortDirection.Descending:
                return "DESC";

            default:
                throw new ArgumentException($"Unknown sort direction: {sortDirection}.");
        }
    }
}
