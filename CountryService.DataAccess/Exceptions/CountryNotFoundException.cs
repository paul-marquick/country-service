using System;

namespace CountryService.DataAccess.Exceptions;

public class CountryNotFoundException : DataAccessException
{
    public CountryNotFoundException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
}
