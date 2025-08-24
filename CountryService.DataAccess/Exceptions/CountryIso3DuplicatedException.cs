using System;

namespace CountryService.DataAccess.Exceptions;

public class CountryIso3DuplicatedException : DataAccessException
{
    public CountryIso3DuplicatedException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
}
