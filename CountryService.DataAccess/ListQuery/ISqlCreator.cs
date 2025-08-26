using System.Collections.Generic;
using System.Data.Common;

namespace CountryService.DataAccess.ListQuery;

public interface ISqlCreator
{
    string CreateQueryWhereClauseSql(List<Filter>? filters);

    void AddQueryWhereClauseParameters(DbCommand dbCommand, List<Filter>? filters);

    string CreateQueryOrderByClauseSql(List<Sort> sorts);
}
