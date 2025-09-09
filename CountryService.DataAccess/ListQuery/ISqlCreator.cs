using System.Collections.Generic;
using System.Data.Common;

namespace CountryService.DataAccess.ListQuery;

/// <summary>
/// Defines methods for generating SQL query components, including WHERE and ORDER BY clauses, and for adding
/// parameters to a database command.
/// </summary>
/// <remarks>This interface is designed to facilitate the creation of SQL query components dynamically  based on
/// provided filters and sorting criteria. It also provides functionality to populate  database commands with the
/// necessary parameters for executing the queries.</remarks>
public interface ISqlCreator
{
    string CreateQueryWhereClauseSql(List<Filter>? filters);

    void AddQueryWhereClauseParameters(DbCommand dbCommand, List<Filter>? filters);

    string CreateQueryOrderByClauseSql(List<Sort> sorts);
}
