using CountryService.DataAccess.Exceptions;
using Microsoft.Data.SqlClient;

namespace CountryService.DataAccess.Test;

[Collection("Database collection")]
public class CountryDataAccessTest(DatabaseFixture databaseFixture)
{
    [Fact]
    public async Task SelectByIso2Async()
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            Name = DatabaseFixture.CreateRandomString(50),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
        };

        using SqlConnection sqlConnection = new(databaseFixture.ConnectionString);
        await sqlConnection.OpenAsync();

        await databaseFixture.CountryDataAccess.InsertAsync(country, sqlConnection);

        Country _country = await databaseFixture.CountryDataAccess.SelectByIso2Async(country.Iso2, sqlConnection);
        Assert.NotNull(_country);
        Assert.Equal(country.Iso2, _country.Iso2);
        Assert.Equal(country.Iso3, _country.Iso3);
        Assert.Equal(country.IsoNumber, _country.IsoNumber);
        Assert.Equal(country.Name, _country.Name);
    }

    [Fact]
    public async Task SelectByIso2Async_NotFound()
    {
        using SqlConnection sqlConnection = new(databaseFixture.ConnectionString);
        await sqlConnection.OpenAsync();

        await Assert.ThrowsAsync<NotFoundException>(() => databaseFixture.CountryDataAccess.SelectByIso2Async(DatabaseFixture.CreateRandomString(2), sqlConnection));
    }

    [Fact]
    public async Task InsertAsync()
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            Name = DatabaseFixture.CreateRandomString(50),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
        };

        using SqlConnection sqlConnection = new(databaseFixture.ConnectionString);
        await sqlConnection.OpenAsync();

        int affectedRows = await databaseFixture.CountryDataAccess.InsertAsync(country, sqlConnection);
        Assert.True(0 < affectedRows);
    }

    [Fact]
    public async Task InsertAsync_Duplication()
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            Name = DatabaseFixture.CreateRandomString(50),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
        };

        using SqlConnection sqlConnection = new(databaseFixture.ConnectionString);
        await sqlConnection.OpenAsync();

        await databaseFixture.CountryDataAccess.InsertAsync(country, sqlConnection);

        await Assert.ThrowsAsync<DuplicationException>(() => databaseFixture.CountryDataAccess.InsertAsync(country, sqlConnection));
    }
}
