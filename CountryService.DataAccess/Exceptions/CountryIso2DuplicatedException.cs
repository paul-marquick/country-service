using System;

namespace CountryService.DataAccess.Exceptions;

public class CountryIso2DuplicatedException : DataAccessException
{
    public CountryIso2DuplicatedException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
}
