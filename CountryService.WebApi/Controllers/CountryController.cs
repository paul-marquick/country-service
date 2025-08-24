using CountryService.DataAccess;
using CountryService.DataAccess.Exceptions;
using CountryService.DataAccess.ListQuery;
using CountryService.Models.Country;
using CountryService.Shared;
using CountryService.WebApi.ListQuery;
using CountryService.WebApi.Problems;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using static System.Net.Mime.MediaTypeNames;

namespace CountryService.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class CountryController(
    ILogger<CountryController> logger,
    IDbConnectionFactory dbConnectionFactory,
    ICountryDataAccess countryDataAccess,
    ProblemDetailsCreator problemDetailsCreator,
    QueryValidator queryValidator,
    QueryReader queryReader) : ControllerBase
{
    [HttpOptions]
    public void Options()
    {
        Response.Headers.Allow = $"{HttpMethod.Options}, {HttpMethod.Head}, {HttpMethod.Get}, {HttpMethod.Post}, {HttpMethod.Put}, {HttpMethod.Patch}, {HttpMethod.Delete}";
        Response.ContentLength = 0;
    }

    // Example
    // /country?offset=0&limit=10&filters=name:like:unite&filters=isonumber:l:1000&sorts=iso2:desc&sorts=name:asc

    [HttpHead]
    [HttpGet]
    public async Task<ActionResult<List<Country>>> SelectCountriesAsync(
        [FromQuery] int offset = Constants.DefaultOffset,
        [FromQuery] int limit = Constants.DefaultLimit,
        [FromQuery] string[]? filters = null,
        [FromQuery] string[]? sorts = null)
    {
        string method = HttpContext.Request.Method;

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug($"CountryController.SelectCountriesAsync, method: {method}, offset: {offset}, limit: {limit}.");

            if (filters == null)
            {
                logger.LogDebug("filters: null");
            }
            else
            {
                logger.LogDebug($"filters: {string.Join(", ", filters)}");
            }

            if (sorts == null)
            {
                logger.LogDebug("sorts: null");
            }
            else
            {
                logger.LogDebug($"sorts: {string.Join(", ", sorts)}");
            }
        }

        queryValidator.Validate(
            ModelState,
            FilterableProperties,
            SortableProperties, 
            GetDataType, 
            offset,
            limit,
            filters,
            sorts);

        if (!ModelState.IsValid)
        {
            return BadRequest(problemDetailsCreator.CreateValidationProblemDetails(HttpContext, ModelState));
        }

        Query query = new Query(offset, limit);
        queryReader.GetFilters(query, filters);
        queryReader.GetSorts(query, sorts, DefaultSortPropertyName, DefaultSortDirection);

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        DbTransaction dbTransaction = await dbConnection.BeginTransactionAsync();

        (int, List<Country>) queryCountriesResult = await countryDataAccess.CountryQueryAsync(query, dbConnection, dbTransaction);

        Response.Headers[AdditionalHeaderNames.Total] = queryCountriesResult.Item1.ToString();

        if (method == HttpMethod.Head.Method)
        {
            Response.Headers.ContentType = Application.Json;
            Response.Headers.ContentLength = queryCountriesResult.Item2.ToString()!.Length;

            return new EmptyResult();
        }
        else
        {
            return Ok(queryCountriesResult.Item2);
        }
    }

    [HttpHead("lookup")]
    [HttpGet("lookup")]
    public async Task<ActionResult<List<CountryLookup>>> GetCountryLookupsAsync()
    {
        string method = HttpContext.Request.Method;

        logger.LogDebug($"GetCountryLookupsAsync, method: {method}");

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        List<CountryLookup> countryLookups = await countryDataAccess.SelectCountryLookupsAsync(dbConnection);

        Response.Headers[AdditionalHeaderNames.Count] = countryLookups.Count.ToString();

        if (method == HttpMethod.Head.Method)
        {
            Response.Headers.ContentType = Application.Json;
            Response.Headers.ContentLength = countryLookups.ToString()!.Length;

            return new EmptyResult();
        }
        else
        {
            return Ok(countryLookups);
        }
    }

    [HttpHead("{iso2}")]
    [HttpGet("{iso2}")]
    public async Task<ActionResult<Country>> GetCountryByIso2Async(string iso2)
    {
        string method = HttpContext.Request.Method;

        logger.LogDebug($"GetCountryByIso2Async, method: {method}, iso2: {iso2}");

        // Just to show how to get the correlation id in code.
        Request.Headers.TryGetValue("x-correlation-id", out var correlationId);
        logger.LogDebug($"correlationId: {correlationId}");
        // Could store the correlation ID in an event table in the database, for example.

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        try
        {
            Country country = await countryDataAccess.SelectCountryByIso2Async(iso2, dbConnection);

            if (method == HttpMethod.Head.Method)
            {
                Response.Headers.ContentType = Application.Json;
                Response.Headers.ContentLength = country.ToString()!.Length;

                return new EmptyResult();
            }
            else
            {
                return Ok(country);
            }
        }
        catch (DataAccessException dataAccessException)
        {
            if (dataAccessException is CountryNotFoundException)
            {
                if (method == HttpMethod.Head.Method)
                {
                    Response.StatusCode = StatusCodes.Status404NotFound;
                    Response.Headers.ContentLength = 0;
                    return new EmptyResult();
                }
                else
                {
                    return NotFound(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status404NotFound,
                            ProblemType.CountryNotFound,
                            ProblemTitle.CountryNotFound,
                            $"Country with iso2: '{iso2}' not found."));
                }
            }
            else
            {
                throw;
            }
        }
    }

    [HttpPost]
    public async Task<ActionResult<Country>> PostCountryAsync([FromBody] Country country)
    {
        logger.LogDebug($"PostCountryAsync, country.Iso2: {country.Iso2}");

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        try
        {
            await countryDataAccess.InsertCountryAsync(country, dbConnection);

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
    public async Task<ActionResult> PutCountryByIso2Async(string iso2, [FromBody] Country country)
    {
        logger.LogDebug($"PutCountryByIso2Async, iso2: {iso2}");

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        DbTransaction dbTransaction = await dbConnection.BeginTransactionAsync();

        try
        {
            // Check row exists.
            await countryDataAccess.SelectCountryByIso2Async(iso2, dbConnection, dbTransaction);

            await countryDataAccess.UpdateCountryByIso2Async(iso2, country, dbConnection, dbTransaction);

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

    //TODO: Validation of the patch document, needs thorough testing. Not sure it is being validated.

    [HttpPatch("{iso2}")]
    public async Task<ActionResult> PatchCountryByIso2Async([FromRoute] string iso2, [FromBody] JsonPatchDocument<Country> countryPatch)
    {
        logger.LogDebug($"PatchCountryByIso2Async, iso2: {iso2}");

        // Currently only supports replace operations.
        if (countryPatch.Operations.Any(x => x.OperationType != Microsoft.AspNetCore.JsonPatch.Operations.OperationType.Replace))
        {
            return BadRequest(
                problemDetailsCreator.CreateProblemDetails(
                    HttpContext,
                    StatusCodes.Status400BadRequest,
                    ProblemType.UnsupportedPatchOperation,
                    ProblemTitle.UnsupportedPatchOperation,
                    $"Unsupported patch operation, currently only replace is supported."));
        }

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        DbTransaction dbTransaction = await dbConnection.BeginTransactionAsync();

        try
        {
            // Check row exists, will throw if not found.
            Country country = await countryDataAccess.SelectCountryByIso2Async(iso2, dbConnection, dbTransaction);

            // Apply the patch.
            countryPatch.ApplyTo(country, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<string> dirtyColumns = countryPatch.Operations.Select(x => x.path).ToList();

            logger.LogDebug($"PatchByIso2Async, dirtyColumns: {dirtyColumns}");

            try
            {
                await countryDataAccess.PartialUpdateCountryByIso2Async(iso2, country, dirtyColumns, dbConnection, dbTransaction);

                await dbTransaction.CommitAsync();

                return NoContent();
            }
            catch (DataAccessException dataAccessException)
            {
                await dbTransaction.RollbackAsync();

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
        catch (DataAccessException dataAccessException)
        {
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

                default:
                    throw;
            }
        }
    }

    [HttpDelete("{iso2}")]
    public async Task<ActionResult> DeleteCountryByIso2Async(string iso2)
    {
        logger.LogDebug($"DeleteCountryByIso2Async, iso2: {iso2}.");

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        await countryDataAccess.DeleteCountryByIso2Async(iso2, dbConnection);

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
    
    private DataType GetDataType(string propertyName)
    {
        logger.LogDebug($"Country.GetDataType, propertyName: {propertyName}.");

        switch (propertyName.ToLower())
        {
            case "iso2":
            case "iso3":
            case "name":
            case "callingcode":
                return DataType.Text;

            case "isonumber":
                return DataType.Numeric;

            default:
                throw new ArgumentException($"Unknown property name: {propertyName}.");
        }
    }

    public static readonly string[] SortableProperties = ["Iso2", "Iso3", "IsoNumber", "Name"];

    public static readonly string[] FilterableProperties = ["Iso2", "Iso3", "IsoNumber", "Name", "CallingCode"];

    public const string DefaultSortPropertyName = "Name";
    public const string DefaultSortDirection = SortDirection.Ascending;
}
