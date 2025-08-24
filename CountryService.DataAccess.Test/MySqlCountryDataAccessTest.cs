using System.Threading.Tasks;

namespace CountryService.DataAccess.Test;

[Collection("Database collection")]
public class MySqlCountryDataAccessTest(DatabaseFixture databaseFixture) : CountryDataAccessTestBase
{
    [Fact]
    public async Task SelectListAsync()
    {
        await _SelectListAsync(databaseFixture.MySqlDbConnectionFactory, databaseFixture.MySqlCountryDataAccess);
    }

    [Fact]
    public async Task SelectLookupListAsync()
    {
        await _SelectLookupListAsync(databaseFixture.MySqlDbConnectionFactory, databaseFixture.MySqlCountryDataAccess);
    }

    [Fact]
    public async Task SelectByIso2Async()
    {
        await _SelectByIso2Async(databaseFixture.MySqlDbConnectionFactory, databaseFixture.MySqlCountryDataAccess);
    }

    [Fact]
    public async Task SelectByIso2Async_NotFound()
    {
        await _SelectByIso2Async_NotFound(databaseFixture.MySqlDbConnectionFactory, databaseFixture.MySqlCountryDataAccess);
    }

    [Fact]
    public async Task InsertAsync()
    {
        await _InsertAsync(databaseFixture.MySqlDbConnectionFactory, databaseFixture.MySqlCountryDataAccess);
    }

    [Fact]
    public async Task InsertAsync_DuplicateIso2()
    {
        await _InsertAsync_DuplicateIso2(databaseFixture.MySqlDbConnectionFactory, databaseFixture.MySqlCountryDataAccess);
    }

    [Fact]
    public async Task UpdateByIso2Async()
    {
        await _UpdateByIso2Async(databaseFixture.MySqlDbConnectionFactory, databaseFixture.MySqlCountryDataAccess);
    }

    [Fact]
    public async Task UpdateByIso2Async_DuplicateName()
    {
        await _UpdateByIso2Async_DuplicateName(databaseFixture.MySqlDbConnectionFactory, databaseFixture.MySqlCountryDataAccess);
    }

    [Fact]
    public async Task DeleteByIso2Async_Delete()
    {
        await _DeleteByIso2Async_Delete(databaseFixture.MySqlDbConnectionFactory, databaseFixture.MySqlCountryDataAccess);
    }

    [Fact]
    public async Task DeleteByIso2Async_NoDelete()
    {
        await _DeleteByIso2Async_NoDelete(databaseFixture.MySqlDbConnectionFactory, databaseFixture.MySqlCountryDataAccess);
    }
}
