using System.Text.RegularExpressions;

namespace CountryService.DataAccess.MySql;

internal static class Utils
{
    public static string? GetConstraintName(string exceptionMessage)
    {
        var match = Regex.Matches(exceptionMessage, @"\b\w*_\w*\b").Cast<Match>().FirstOrDefault();

        return match != null ? match.Value : null;
    }
}
