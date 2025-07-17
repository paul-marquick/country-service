using CountryService.DataAccess.Exceptions;
using CountryService.DataAccess.Models.Country;
using Microsoft.Data.SqlClient;

namespace CountryService.DataAccess.Test;

[Collection("Database collection")]
public class CountryDataAccessTest(DatabaseFixture databaseFixture)
{
    [Fact]
    public async Task SelectListAsync()
    {
        string country1_iso2 = DatabaseFixture.CreateRandomString(2);
        Country country1 = new()
        {
            Iso2 = country1_iso2,
            Iso3 = country1_iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        string country2_iso2 = DatabaseFixture.CreateRandomString(2);
        Country country2 = new()
        {
            Iso2 = country2_iso2,
            Iso3 = country2_iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        string country3_iso2 = DatabaseFixture.CreateRandomString(2);
        Country country3 = new()
        {
            Iso2 = country3_iso2,
            Iso3 = country3_iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        using SqlConnection sqlConnection = new(databaseFixture.ConnectionString);
        await sqlConnection.OpenAsync();

        await databaseFixture.CountryDataAccess.InsertAsync(country1, sqlConnection);
        await databaseFixture.CountryDataAccess.InsertAsync(country2, sqlConnection);
        await databaseFixture.CountryDataAccess.InsertAsync(country3, sqlConnection);

        List<Country> countryList = await databaseFixture.CountryDataAccess.SelectListAsync(sqlConnection);
        Assert.NotNull(countryList);
        Assert.NotEmpty(countryList);
        Assert.True(3 <= countryList.Count);
    }

    [Fact]
    public async Task SelectLookupListAsync()
    {
        string country1_iso2 = DatabaseFixture.CreateRandomString(2);
        Country country1 = new()
        {
            Iso2 = country1_iso2,
            Iso3 = country1_iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        string country2_iso2 = DatabaseFixture.CreateRandomString(2);
        Country country2 = new()
        {
            Iso2 = country2_iso2,
            Iso3 = country2_iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        string country3_iso2 = DatabaseFixture.CreateRandomString(2);
        Country country3 = new()
        {
            Iso2 = country3_iso2,
            Iso3 = country3_iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        using SqlConnection sqlConnection = new(databaseFixture.ConnectionString);
        await sqlConnection.OpenAsync();

        await databaseFixture.CountryDataAccess.InsertAsync(country1, sqlConnection);
        await databaseFixture.CountryDataAccess.InsertAsync(country2, sqlConnection);
        await databaseFixture.CountryDataAccess.InsertAsync(country3, sqlConnection);

        List<CountryLookup> countryLookupList = await databaseFixture.CountryDataAccess.SelectLookupListAsync(sqlConnection);
        Assert.NotNull(countryLookupList);
        Assert.NotEmpty(countryLookupList);
        Assert.True(3 <= countryLookupList.Count);
    }

    [Fact]
    public async Task SelectByIso2Async()
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
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

        await Assert.ThrowsAsync<CountryNotFoundException>(() => databaseFixture.CountryDataAccess.SelectByIso2Async(DatabaseFixture.CreateRandomString(2), sqlConnection));
    }

    [Fact]
    public async Task InsertAsync()
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        using SqlConnection sqlConnection = new(databaseFixture.ConnectionString);
        await sqlConnection.OpenAsync();

        int affectedRows = await databaseFixture.CountryDataAccess.InsertAsync(country, sqlConnection);
        Assert.Equal(1, affectedRows);
    }

    [Fact]
    public async Task InsertAsync_DuplicateIso2()
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country1 = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        using SqlConnection sqlConnection = new(databaseFixture.ConnectionString);
        await sqlConnection.OpenAsync();

        await databaseFixture.CountryDataAccess.InsertAsync(country1, sqlConnection);

        Country country2 = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        await Assert.ThrowsAsync<CountryIso2DuplicatedException>(() => databaseFixture.CountryDataAccess.InsertAsync(country2, sqlConnection));
    }

    [Fact]
    public async Task UpdateByIso2Async()
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        using SqlConnection sqlConnection = new(databaseFixture.ConnectionString);
        await sqlConnection.OpenAsync();

        await databaseFixture.CountryDataAccess.InsertAsync(country, sqlConnection);

        country.Name = DatabaseFixture.CreateRandomString(50);

        int affectedRows = await databaseFixture.CountryDataAccess.UpdateByIso2Async(iso2, country, sqlConnection);
        Assert.Equal(1, affectedRows);
    }

    [Fact]
    public async Task UpdateByIso2Async_DuplicateName()
    {
        string country1Iso2 = DatabaseFixture.CreateRandomString(2);

        Country country1 = new()
        {
            Iso2 = country1Iso2,
            Iso3 = country1Iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        string country2Iso2 = DatabaseFixture.CreateRandomString(2);

        Country country2 = new()
        {
            Iso2 = country2Iso2,
            Iso3 = country2Iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        using SqlConnection sqlConnection = new(databaseFixture.ConnectionString);
        await sqlConnection.OpenAsync();

        await databaseFixture.CountryDataAccess.InsertAsync(country1, sqlConnection);

        await databaseFixture.CountryDataAccess.InsertAsync(country2, sqlConnection);

        // Set country 2 name same as country 1.
        country2.Name = country1.Name;

        await Assert.ThrowsAsync<CountryNameDuplicatedException>(() => databaseFixture.CountryDataAccess.UpdateByIso2Async(country2Iso2, country2, sqlConnection));
    }

    [Fact]
    public async Task DeleteByIso2Async_Deletes()
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        using SqlConnection sqlConnection = new(databaseFixture.ConnectionString);
        await sqlConnection.OpenAsync();

        await databaseFixture.CountryDataAccess.InsertAsync(country, sqlConnection);

        int affectedRows = await databaseFixture.CountryDataAccess.DeleteByIso2Async(iso2, sqlConnection);
        Assert.Equal(1, affectedRows);
    }

    [Fact]
    public async Task DeleteByIso2Async_NoDelete()
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        using SqlConnection sqlConnection = new(databaseFixture.ConnectionString);
        await sqlConnection.OpenAsync();

        await databaseFixture.CountryDataAccess.InsertAsync(country, sqlConnection);

        int affectedRows = await databaseFixture.CountryDataAccess.DeleteByIso2Async(DatabaseFixture.CreateRandomString(2), sqlConnection);
        Assert.Equal(0, affectedRows);
    }
}
