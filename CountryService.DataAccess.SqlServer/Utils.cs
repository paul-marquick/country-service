using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace CountryService.DataAccess.SqlServer;

internal static class Utils
{
    public static string? GetConstraintName(ILogger logger, string exceptionMessage)
    {
        logger.LogDebug($"GetConstraintName, exceptionMessage: {exceptionMessage}");

        var match = Regex.Matches(exceptionMessage, @"\b\w*_\w*\b").Cast<Match>().FirstOrDefault();

        return match != null ? match.Value : null;
    }
}
