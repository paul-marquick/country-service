namespace CountryService.DataAccess.Test;

[Collection("Database collection")]
public class PostgreSqlCountryDataAccessTest(DatabaseFixture databaseFixture) : CountryDataAccessTestBase
{
    [Fact]
    public async Task SelectListAsync()
    {
        await _SelectListAsync(databaseFixture.PostgreSqlDbConnectionFactory, databaseFixture.PostgreSqlCountryDataAccess);
    }

    [Fact]
    public async Task SelectLookupListAsync()
    {
        await _SelectLookupListAsync(databaseFixture.PostgreSqlDbConnectionFactory, databaseFixture.PostgreSqlCountryDataAccess);
    }

    [Fact]
    public async Task SelectByIso2Async()
    {
        await _SelectByIso2Async(databaseFixture.PostgreSqlDbConnectionFactory, databaseFixture.PostgreSqlCountryDataAccess);
    }

    [Fact]
    public async Task SelectByIso2Async_NotFound()
    {
        await _SelectByIso2Async_NotFound(databaseFixture.PostgreSqlDbConnectionFactory, databaseFixture.PostgreSqlCountryDataAccess);
    }

    [Fact]
    public async Task InsertAsync()
    {
        await _InsertAsync(databaseFixture.PostgreSqlDbConnectionFactory, databaseFixture.PostgreSqlCountryDataAccess);
    }

    [Fact]
    public async Task InsertAsync_DuplicateIso2()
    {
        await _InsertAsync_DuplicateIso2(databaseFixture.PostgreSqlDbConnectionFactory, databaseFixture.PostgreSqlCountryDataAccess);
    }

    [Fact]
    public async Task UpdateByIso2Async()
    {
        await _UpdateByIso2Async(databaseFixture.PostgreSqlDbConnectionFactory, databaseFixture.PostgreSqlCountryDataAccess);
    }

    [Fact]
    public async Task UpdateByIso2Async_DuplicateName()
    {
        await _UpdateByIso2Async_DuplicateName(databaseFixture.PostgreSqlDbConnectionFactory, databaseFixture.PostgreSqlCountryDataAccess);
    }

    [Fact]
    public async Task DeleteByIso2Async_Delete()
    {
        await _DeleteByIso2Async_Delete(databaseFixture.PostgreSqlDbConnectionFactory, databaseFixture.PostgreSqlCountryDataAccess);
    }

    [Fact]
    public async Task DeleteByIso2Async_NoDelete()
    {
        await _DeleteByIso2Async_NoDelete(databaseFixture.PostgreSqlDbConnectionFactory, databaseFixture.PostgreSqlCountryDataAccess);
    }
}
