namespace CountryService.DataAccess.Test;

[Collection("Database collection")]
public class SqlServerCountryDataAccessTest(DatabaseFixture databaseFixture) : CountryDataAccessTestBase
{
    [Fact]
    public async Task SelectListAsync()
    {
        await _SelectListAsync(databaseFixture.SqlServerDbConnectionFactory, databaseFixture.SqlServerCountryDataAccess);
    }

    [Fact]
    public async Task SelectLookupListAsync()
    {
        await _SelectLookupListAsync(databaseFixture.SqlServerDbConnectionFactory, databaseFixture.SqlServerCountryDataAccess);
    }

    [Fact]
    public async Task SelectByIso2Async()
    {
        await _SelectByIso2Async(databaseFixture.SqlServerDbConnectionFactory, databaseFixture.SqlServerCountryDataAccess);
    }

    [Fact]
    public async Task SelectByIso2Async_NotFound()
    {
        await _SelectByIso2Async_NotFound(databaseFixture.SqlServerDbConnectionFactory, databaseFixture.SqlServerCountryDataAccess);
    }

    [Fact]
    public async Task InsertAsync()
    {
        await _InsertAsync(databaseFixture.SqlServerDbConnectionFactory, databaseFixture.SqlServerCountryDataAccess);
    }

    [Fact]
    public async Task InsertAsync_DuplicateIso2()
    {
        await _InsertAsync_DuplicateIso2(databaseFixture.SqlServerDbConnectionFactory, databaseFixture.SqlServerCountryDataAccess);
    }

    [Fact]
    public async Task UpdateByIso2Async()
    {
        await _UpdateByIso2Async(databaseFixture.SqlServerDbConnectionFactory, databaseFixture.SqlServerCountryDataAccess);
    }

    [Fact]
    public async Task UpdateByIso2Async_DuplicateName()
    {
        await _UpdateByIso2Async_DuplicateName(databaseFixture.SqlServerDbConnectionFactory, databaseFixture.SqlServerCountryDataAccess);
    }

    [Fact]
    public async Task DeleteByIso2Async_Delete()
    {
        await _DeleteByIso2Async_Delete(databaseFixture.SqlServerDbConnectionFactory, databaseFixture.SqlServerCountryDataAccess);
    }

    [Fact]
    public async Task DeleteByIso2Async_NoDelete()
    {
        await _DeleteByIso2Async_NoDelete(databaseFixture.SqlServerDbConnectionFactory, databaseFixture.SqlServerCountryDataAccess);
    }
}
