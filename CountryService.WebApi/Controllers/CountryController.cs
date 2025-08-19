using CountryService.DataAccess;
using CountryService.DataAccess.Exceptions;
using CountryService.DataAccess.ListQuery;
using CountryService.DataAccess.Models.Country;
using CountryService.Shared;
using CountryService.WebApi.Problems;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;

namespace CountryService.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class CountryController(
    ILogger<CountryController> logger, 
    IDbConnectionFactory dbConnectionFactory, 
    ICountryDataAccess countryDataAccess, 
    ProblemDetailsCreator problemDetailsCreator) : ControllerBase
{
    [HttpOptions]
    public void Options()
    {
        Response.Headers.Allow = $"{HttpMethod.Options}, {HttpMethod.Head}, {HttpMethod.Get}, {HttpMethod.Post}, {HttpMethod.Put}, {HttpMethod.Patch}, {HttpMethod.Delete}";
        Response.ContentLength = 0;
    }

    [HttpGet("throw")]
    public IActionResult Throw()
    {
        // Just to show how to get the correlation id in code.
        Request.Headers.TryGetValue("x-correlation-id", out var correlationId);
        logger.LogDebug($"correlationId: {correlationId}");

        // Could store the correlation ID in an event table in the database, for example.

        throw new Exception("Sample exception.");
    }

    // Example

    // /country?offset=0&limit=10&filters=name:like:engla&filters=isonumber:lessthan:1000&sorts=iso2:desc&sorts=name:asc

    [HttpHead("query")]
    [HttpGet("query")]
    public async Task<ActionResult<List<Country>>> QueryCountriesAsync(
        [FromQuery] int offset = 0, 
        [FromQuery] int limit = 10, 
        [FromQuery] string[]? filters = null, 
        [FromQuery] string[]? sorts = null)
    {
        string method = HttpContext.Request.Method;

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug($"method: {method}, offset: {offset}, limit: {limit}.");

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

        if (offset < 0)
        {
            ModelState.AddModelError("offset", "offset must be greater or equal to 0.");
        }

        if (!Constants.ValidLimits.Contains(limit))
        {
            ModelState.AddModelError("limit", "limit must be 10, 20, 50 or 100.");
        }

        ValidateQueryFilters(filters);
        ValidateQuerySorts(sorts);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Query query = new Query(offset, limit);
        query.AddSort("Name", SortDirection.Ascending);
        query.AddFilter("IsoNumber", ComparisonOperator.LessThan, 1000);

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        DbTransaction dbTransaction = await dbConnection.BeginTransactionAsync();

        (int, List<Country>) queryCountriesResult = await countryDataAccess.CountryQueryAsync(query, dbConnection, dbTransaction);

        Response.Headers["Total"] = queryCountriesResult.Item1.ToString();

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

    private void ValidateQueryFilters(string[]? filters)
    {
        if (filters != null && filters.Length > 0)
        {
            foreach (string f in filters)
            {
                // Example filter - name:like:fred

                // Split on colon.
                string[] filterParts = f.Split(':');
                string propertyName = filterParts[0];
                string comparisonOperator = filterParts[1];
                string value = string.Join(':', filterParts, 2, filterParts.Length - 1);

                // Check the property name is filterable.
                if (!CountryMetaData.FilterableProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("filters", "Property is invalid.");
                }

                // Check the comparison operator is valid.
                if (!comparisonOperator.Equals(ComparisonOperator.EqualTo, StringComparison.OrdinalIgnoreCase) &&
                    !comparisonOperator.Equals(ComparisonOperator.NotEqualTo, StringComparison.OrdinalIgnoreCase) &&
                    !comparisonOperator.Equals(ComparisonOperator.GreaterThan, StringComparison.OrdinalIgnoreCase) &&
                    !comparisonOperator.Equals(ComparisonOperator.LessThan, StringComparison.OrdinalIgnoreCase) &&
                    !comparisonOperator.Equals(ComparisonOperator.GreaterThanOrEqualTo, StringComparison.OrdinalIgnoreCase) &&
                    !comparisonOperator.Equals(ComparisonOperator.LessThanOrEqualTo, StringComparison.OrdinalIgnoreCase) &&
                    !comparisonOperator.Equals(ComparisonOperator.Like, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("filters", "Comparison operator is invalid.");
                }

                // Check the property data type and comparison operator are compatible.
                if (!ComparisonOperatorDbType.IsComparisonOperatorForDataType(comparisonOperator, CountryMetaData.GetDataType(propertyName)))
                {
                    ModelState.AddModelError("filters", "Comparison operator is not compatible with the data type of the specified property.");
                }
            }
        }
    }

    private void ValidateQuerySorts(string[]? sorts)
    {
        if (sorts != null && sorts.Length > 0)
        {
            foreach (string s in sorts)
            {
                // Example: iso2:desc

                // Split on colon.
                string[] sortParts = s.Split(':');
                string propertyName = sortParts[0];
                string sortDirection = sortParts[1];

                // Check the property name is sortable.
                if (!CountryMetaData.SortableProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("sorts", "Property is invalid.");
                }

                // Check the sort direction is valid.
                if (!sortDirection.Equals(SortDirection.Ascending, StringComparison.OrdinalIgnoreCase) &&
                    !sortDirection.Equals(SortDirection.Descending, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("sorts", "Sort direction is invalid.");
                }
            }
        }
    }

    [HttpHead]
    [HttpGet]
    public async Task<ActionResult<List<Country>>> GetCountriesAsync()
    {
        string method = HttpContext.Request.Method;

        logger.LogDebug($"GetCountriesAsync, method: {method}");

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        List<Country> countries = await countryDataAccess.SelectCountriesAsync(dbConnection);

        if (method == HttpMethod.Head.Method)
        {
            Response.Headers.ContentType = Application.Json;
            Response.Headers.ContentLength = countries.ToString()!.Length;

            return new EmptyResult();
        }
        else
        {
            return Ok(countries);
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
}
