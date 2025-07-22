using Microsoft.AspNetCore.Mvc;
using CountryService.DataAccess;
using Microsoft.Data.SqlClient;
using CountryService.DataAccess.Exceptions;
using System.Net;
using CountryService.DataAccess.Models.Country;

namespace CountryService.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class CountryController(
    ILogger<CountryController> logger, 
    DatabaseOptions databaseOptions, 
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

        using SqlConnection sqlConnection = new(databaseOptions.ConnectionString);
        await sqlConnection.OpenAsync();

        return Ok(await countryDataAccess.SelectListAsync(sqlConnection));
    }

    [HttpGet("{iso2}")]
    public async Task<ActionResult<Country>> GetByIso2Async(string iso2)
    {
        logger.LogDebug($"GetByIso2Async, iso2: {iso2}");

        // Just to show how to get the correlation id in code. NOT WORKING.
        Request.Headers.TryGetValue("x-correlation-id", out var correlationId);
        logger.LogDebug($"x-correlation-id: {correlationId}");

        using SqlConnection sqlConnection = new(databaseOptions.ConnectionString);
        await sqlConnection.OpenAsync();

        try
        {
            Country country = await countryDataAccess.SelectByIso2Async(iso2, sqlConnection);

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

        using SqlConnection sqlConnection = new(databaseOptions.ConnectionString);
        await sqlConnection.OpenAsync();

        try
        {
            await countryDataAccess.InsertAsync(country, sqlConnection);

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

        using SqlConnection sqlConnection = new(databaseOptions.ConnectionString);
        await sqlConnection.OpenAsync();

        try
        {
            // Check row exists.
            await countryDataAccess.SelectByIso2Async(iso2, sqlConnection);

            await countryDataAccess.UpdateByIso2Async(iso2, country, sqlConnection);

            return NoContent();
        }
        catch (DataAccessException dataAccessException)
        {
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

        using SqlConnection sqlConnection = new(databaseOptions.ConnectionString);
        await sqlConnection.OpenAsync();

        await countryDataAccess.DeleteByIso2Async(iso2, sqlConnection);

        return NoContent();
    }
}
