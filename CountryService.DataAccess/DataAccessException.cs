namespace CountryService.DataAccess;

public class DataAccessException : Exception
{
    public DataAccessException(DataAccessExceptionType type, string? message = null, Exception? innerException = null) : base(message, innerException) 
    {
        Type = type;
    }

    public DataAccessExceptionType Type { get; set; }
}
