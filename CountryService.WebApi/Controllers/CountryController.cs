using CountryService.DataAccess;
using CountryService.DataAccess.Exceptions;
using CountryService.DataAccess.Models.Country;
using CountryService.WebApi.Problems;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace CountryService.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class CountryController(
    ILogger<CountryController> logger, 
    IDbConnectionFactory dbConnectionFactory, 
    ICountryDataAccess countryDataAccess, 
    ProblemDetailsCreator problemDetailsCreator) : ControllerBase
{
    [HttpGet("Throw")]
    public IActionResult Throw()
    {
        // Just to show how to get the request id in code.
        Request.Headers.TryGetValue("x-correlation-id", out var correlationId);
        logger.LogDebug($"x-correlation-id: {correlationId}");

        // Could store the correlation ID in an event table in the database, for example.

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

        // Just to show how to get the request id in code.
        Request.Headers.TryGetValue("x-correlation-id", out var correlationId);
        logger.LogDebug($"x-correlation-id: {correlationId}");

        // Could store the correlation ID in an event table in the database, for example.

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
                return NotFound(
                    problemDetailsCreator.CreateProblemDetails(
                        HttpContext, 
                        StatusCodes.Status404NotFound,
                        ProblemType.CountryNotFound,
                        ProblemTitle.CountryNotFound,
                        $"Country with iso2: '{iso2}' not found."));
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
                case CountryIso2DuplicatedException countryIso2DuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            ProblemType.CountryIso2Duplicated,
                            ProblemTitle.CountryIso2Duplicated,
                            $"Country has iso2: '{country.Iso2}' duplicated."));

                case CountryIso3DuplicatedException countryIso3DuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            ProblemType.CountryIso3Duplicated,
                            ProblemTitle.CountryIso3Duplicated,
                            $"Country has iso3: '{country.Iso3}' duplicated."));

                case CountryIsoNumberDuplicatedException countryIsoNumberDuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            ProblemType.CountryIsoNumberDuplicated,
                            ProblemTitle.CountryIsoNumberDuplicated,
                            $"Country has isoNumber: '{country.IsoNumber}' duplicated."));

                case CountryNameDuplicatedException countryNameDuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            ProblemType.CountryNameDuplicated,
                            ProblemTitle.CountryNameDuplicated,
                            $"Country has name: '{country.Name}' duplicated."));

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
                case CountryNotFoundException countryNotFoundException:
                    return NotFound(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status404NotFound,
                            ProblemType.CountryNotFound,
                            ProblemTitle.CountryNotFound,
                            $"Country with iso2: '{iso2}' not found."));

                case CountryIso2DuplicatedException countryIso2DuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            ProblemType.CountryIso2Duplicated,
                            ProblemTitle.CountryIso2Duplicated,
                            $"Country has iso2: '{country.Iso2}' duplicated."));

                case CountryIso3DuplicatedException countryIso3DuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            ProblemType.CountryIso3Duplicated,
                            ProblemTitle.CountryIso3Duplicated,
                            $"Country has iso3: '{country.Iso3}' duplicated."));

                case CountryIsoNumberDuplicatedException countryIsoNumberDuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            ProblemType.CountryIsoNumberDuplicated,
                            ProblemTitle.CountryIsoNumberDuplicated,
                            $"Country has isoNumber: '{country.IsoNumber}' duplicated."));

                case CountryNameDuplicatedException countryNameDuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            ProblemType.CountryNameDuplicated,
                            ProblemTitle.CountryNameDuplicated,
                            $"Country has name: '{country.Name}' duplicated."));

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
