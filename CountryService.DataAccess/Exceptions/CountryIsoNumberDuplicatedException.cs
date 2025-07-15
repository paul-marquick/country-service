namespace CountryService.DataAccess.Exceptions;

public class CountryIsoNumberDuplicatedException : DataAccessException
{
    public CountryIsoNumberDuplicatedException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
}
