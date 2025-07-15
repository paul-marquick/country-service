using Microsoft.Extensions.Logging;
using Moq;

namespace CountryService.DataAccess.Test;

public class DatabaseFixture
{
    public DatabaseFixture() 
    {
        ConnectionString = "Server=.;Database=CountryService;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true";

        CountryDataAccess = new SqlServer.CountryDataAccess(Mock.Of<ILogger<SqlServer.CountryDataAccess>>());
    }

    public string ConnectionString { get; private set; }

    public ICountryDataAccess CountryDataAccess { get; private set; }

    public static string CreateRandomString(int length)
    {
        Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static int CreateRandomInt32()
    {
        return new Random().Next(int.MaxValue);
    }
}
