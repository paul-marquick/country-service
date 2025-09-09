using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.RegularExpressions;

namespace CountryService.DataAccess.SqlServer;

/// <summary>
/// Provides utility methods for working with SQL-related operations.
/// </summary>
/// <remarks>This class contains static methods designed to assist with common SQL-related tasks,  such as
/// extracting constraint names from exception messages. It is intended for internal use only.</remarks>
internal static class SqlUtils
{
    public static string? GetConstraintName(ILogger logger, string exceptionMessage)
    {
        logger.LogDebug($"GetConstraintName, exceptionMessage: {exceptionMessage}");

        var match = Regex.Matches(exceptionMessage, @"\b\w*_\w*\b").Cast<Match>().FirstOrDefault();

        return match != null ? match.Value : null;
    }
}
