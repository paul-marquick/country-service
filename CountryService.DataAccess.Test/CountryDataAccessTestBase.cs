using CountryService.DataAccess.Exceptions;
using CountryService.Models.Country;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace CountryService.DataAccess.Test;

public abstract class CountryDataAccessTestBase()
{
    protected async Task _SelectListAsync(IDbConnectionFactory dbConnectionFactory, ICountryDataAccess countryDataAccess)
    {
        string country1_iso2 = DatabaseFixture.CreateRandomString(2);
        Country country1 = new()
        {
            Iso2 = country1_iso2,
            Iso3 = country1_iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50),
            CallingCode = DatabaseFixture.CreateRandomString(50)
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
            Name = DatabaseFixture.CreateRandomString(50),
            CallingCode = DatabaseFixture.CreateRandomString(50)
        };

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        await countryDataAccess.InsertCountryAsync(country1, dbConnection);
        await countryDataAccess.InsertCountryAsync(country2, dbConnection);
        await countryDataAccess.InsertCountryAsync(country3, dbConnection);

        List<Country> countryList = await countryDataAccess.SelectCountriesAsync(dbConnection);
        Assert.NotNull(countryList);
        Assert.NotEmpty(countryList);
        Assert.True(3 <= countryList.Count);
    }

    protected async Task _SelectLookupListAsync(IDbConnectionFactory dbConnectionFactory, ICountryDataAccess countryDataAccess)
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
            Name = DatabaseFixture.CreateRandomString(50),
            CallingCode = DatabaseFixture.CreateRandomString(50)
        };

        string country3_iso2 = DatabaseFixture.CreateRandomString(2);
        Country country3 = new()
        {
            Iso2 = country3_iso2,
            Iso3 = country3_iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        await countryDataAccess.InsertCountryAsync(country1, dbConnection);
        await countryDataAccess.InsertCountryAsync(country2, dbConnection);
        await countryDataAccess.InsertCountryAsync(country3, dbConnection);

        List<CountryLookup> countryLookupList = await countryDataAccess.SelectCountryLookupsAsync(dbConnection);
        Assert.NotNull(countryLookupList);
        Assert.NotEmpty(countryLookupList);
        Assert.True(3 <= countryLookupList.Count);
    }

    protected async Task _SelectByIso2Async(IDbConnectionFactory dbConnectionFactory, ICountryDataAccess countryDataAccess)
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50),
            CallingCode = DatabaseFixture.CreateRandomString(50)
        };

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        await countryDataAccess.InsertCountryAsync(country, dbConnection);

        Country _country = await countryDataAccess.SelectCountryByIso2Async(country.Iso2, dbConnection);
        Assert.NotNull(_country);
        Assert.Equal(country.Iso2, _country.Iso2);
        Assert.Equal(country.Iso3, _country.Iso3);
        Assert.Equal(country.IsoNumber, _country.IsoNumber);
        Assert.Equal(country.Name, _country.Name);
    }

    protected async Task _SelectByIso2Async_NotFound(IDbConnectionFactory dbConnectionFactory, ICountryDataAccess countryDataAccess)
    {
        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        await Assert.ThrowsAsync<CountryNotFoundException>(() => countryDataAccess.SelectCountryByIso2Async(DatabaseFixture.CreateRandomString(2), dbConnection));
    }

    protected async Task _InsertAsync(IDbConnectionFactory dbConnectionFactory, ICountryDataAccess countryDataAccess)
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        int affectedRows = await countryDataAccess.InsertCountryAsync(country, dbConnection);
        Assert.Equal(1, affectedRows);
    }

    protected async Task _InsertAsync_DuplicateIso2(IDbConnectionFactory dbConnectionFactory, ICountryDataAccess countryDataAccess)
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country1 = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50),
            CallingCode = DatabaseFixture.CreateRandomString(50)
        };

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        await countryDataAccess.InsertCountryAsync(country1, dbConnection);

        Country country2 = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        await Assert.ThrowsAsync<CountryIso2DuplicatedException>(() => countryDataAccess.InsertCountryAsync(country2, dbConnection));
    }

    protected async Task _UpdateByIso2Async(IDbConnectionFactory dbConnectionFactory, ICountryDataAccess countryDataAccess)
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50),
            CallingCode = DatabaseFixture.CreateRandomString(50)
        };

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        await countryDataAccess.InsertCountryAsync(country, dbConnection);

        country.Name = DatabaseFixture.CreateRandomString(50);

        int affectedRows = await countryDataAccess.UpdateCountryByIso2Async(iso2, country, dbConnection);
        Assert.Equal(1, affectedRows);
    }

    protected async Task _UpdateByIso2Async_DuplicateName(IDbConnectionFactory dbConnectionFactory, ICountryDataAccess countryDataAccess)
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
            Name = DatabaseFixture.CreateRandomString(50),
            CallingCode = DatabaseFixture.CreateRandomString(50)
        };

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        await countryDataAccess.InsertCountryAsync(country1, dbConnection);

        await countryDataAccess.InsertCountryAsync(country2, dbConnection);

        // Set country 2 name same as country 1.
        country2.Name = country1.Name;

        await Assert.ThrowsAsync<CountryNameDuplicatedException>(() => countryDataAccess.UpdateCountryByIso2Async(country2Iso2, country2, dbConnection));
    }

    protected async Task _DeleteByIso2Async_Delete(IDbConnectionFactory dbConnectionFactory, ICountryDataAccess countryDataAccess)
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50)
        };

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        await countryDataAccess.InsertCountryAsync(country, dbConnection);

        int affectedRows = await countryDataAccess.DeleteCountryByIso2Async(iso2, dbConnection);
        Assert.Equal(1, affectedRows);
    }

    protected async Task _DeleteByIso2Async_NoDelete(IDbConnectionFactory dbConnectionFactory, ICountryDataAccess countryDataAccess)
    {
        string iso2 = DatabaseFixture.CreateRandomString(2);

        Country country = new()
        {
            Iso2 = iso2,
            Iso3 = iso2 + DatabaseFixture.CreateRandomString(1),
            IsoNumber = DatabaseFixture.CreateRandomInt32(),
            Name = DatabaseFixture.CreateRandomString(50),
            CallingCode = DatabaseFixture.CreateRandomString(50)
        };

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        await countryDataAccess.InsertCountryAsync(country, dbConnection);

        int affectedRows = await countryDataAccess.DeleteCountryByIso2Async(DatabaseFixture.CreateRandomString(2), dbConnection);
        Assert.Equal(0, affectedRows);
    }
}
