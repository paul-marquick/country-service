namespace CountryService.DataAccess.ListQuery;

public interface ISortDirectionConverter
{
    string GetSortDirectionSql(string sortDirection);
}
