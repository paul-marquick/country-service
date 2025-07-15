namespace CountryService.DataAccess.Exceptions;

public class CountryNameDuplicatedException : DataAccessException
{
    public CountryNameDuplicatedException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
}
