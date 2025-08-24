using System;

namespace CountryService.DataAccess.Exceptions;

public class DataAccessException : Exception
{
    public DataAccessException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
}
