using System;
using Microsoft.Extensions.Logging;

namespace CountryService.DataAccess.MySql;

internal static class Utils
{
    // Example exceptionMessage: Duplicate entry 'CX' for key 'country.PRIMARY'

    public static DataExceptionType GetDataExceptionType(ILogger logger, string exceptionMessage)
    {
        logger.LogDebug($"GetDataExceptionType, exceptionMessage: {exceptionMessage}");

        if (exceptionMessage.Contains("Duplicate entry"))
        {
            return DataExceptionType.Duplication;
        }
        else
        {
            logger.LogError($"GetDataExceptionType, unable to get data exception type from exceptionMessage: {exceptionMessage}");

            return DataExceptionType.Unknown;
        }
    }

    public static string? GetConstraintName(ILogger logger, string exceptionMessage)
    {
        logger.LogDebug($"GetConstraintName, exceptionMessage: {exceptionMessage}");

        try
        {
            var parts = exceptionMessage.Split("'");

            return parts[parts.Length - 2];
        }
        catch (Exception exception)
        {
            logger.LogError($"GetConstraintName, exception throw trying to extract constraint name. exceptionMessage: {exceptionMessage}", exception);

            throw;
        }
    }
}
