using CountryService.DataAccess;
using CountryService.DataAccess.Exceptions;
using CountryService.DataAccess.Models.Country;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Net;

namespace CountryService.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class CountryController(
    ILogger<CountryController> logger, 
    IDbConnectionFactory dbConnectionFactory, 
    ICountryDataAccess countryDataAccess) : ControllerBase
{
    [HttpGet("Throw")]
    public IActionResult Throw()
    {
        throw new Exception("Sample exception.");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Country>>> GetCountryListAsync()
    {
        logger.LogDebug("GetCountryListAsync");

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        return Ok(await countryDataAccess.SelectListAsync(dbConnection));
    }

    [HttpGet("{iso2}")]
    public async Task<ActionResult<Country>> GetByIso2Async(string iso2)
    {
        logger.LogDebug($"GetByIso2Async, iso2: {iso2}");

        // Just to show how to get the correlation id in code. NOT WORKING.
        Request.Headers.TryGetValue("x-correlation-id", out var correlationId);
        logger.LogDebug($"x-correlation-id: {correlationId}");

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        try
        {
            Country country = await countryDataAccess.SelectByIso2Async(iso2, dbConnection);

            return Ok(country);
        }
        catch (DataAccessException dataAccessException)
        {
            if (dataAccessException is CountryNotFoundException)
            {
                throw new ProblemDetailsException(HttpStatusCode.NotFound, "Country not found", $"Country with iso2: '{iso2}' not found.");
            }
            else
            {
                throw;
            }
        }
    }

    [HttpPost]
    public async Task<ActionResult<Country>> PostAsync([FromBody] Country country)
    {
        logger.LogDebug($"PostAsync, country.Iso2: {country.Iso2}");

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        try
        {
            await countryDataAccess.InsertAsync(country, dbConnection);

            return Created($"https://api.example.com/country/{country.Iso2}", country);
        }
        catch (DataAccessException dataAccessException)
        {
            switch (dataAccessException)
            {
                case CountryIso2DuplicatedException ex:
                    return BadRequest();

                default:
                    throw;
            }
        }
    }

    [HttpPut("{iso2}")]
    public async Task<ActionResult> PutByIso2Async(string iso2, [FromBody] Country country)
    {
        logger.LogDebug($"PutByIso2Async, iso2: {iso2}");

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        DbTransaction? dbTransaction = await dbConnection.BeginTransactionAsync();

        try
        {         
            // Check row exists.
            await countryDataAccess.SelectByIso2Async(iso2, dbConnection, dbTransaction);

            await countryDataAccess.UpdateByIso2Async(iso2, country, dbConnection, dbTransaction);

            await dbTransaction.CommitAsync();

            return NoContent();
        }
        catch (DataAccessException dataAccessException)
        {
            await dbTransaction.RollbackAsync();

            switch (dataAccessException)
            {
                case CountryNotFoundException ex:
                    return NotFound();

                case CountryIso2DuplicatedException ex:
                    return BadRequest();

                default:
                    throw;
            }
        }
    }

    [HttpDelete("{iso2}")]
    public async Task<ActionResult> DeleteByIso2Async(string iso2)
    {
        logger.LogDebug($"DeleteByIso2Async, iso2: {iso2}.");

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        await countryDataAccess.DeleteByIso2Async(iso2, dbConnection);

        return NoContent();
    }

    [HttpGet]
    [Route("does-country-name-exist/{name}/{iso2}")]
    [Route("does-country-name-exist/{name}")]
    public async Task<ActionResult<bool>> DoesCountryNameExistAsync(string name, string? iso2)
    {
        logger.LogDebug($"DoesCountryNameExistAsync, name: {name}, iso2: {iso2}");

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        bool exists = await countryDataAccess.DoesCountryNameExistAsync(name, iso2, dbConnection);

        return Ok(exists);
    }
}
