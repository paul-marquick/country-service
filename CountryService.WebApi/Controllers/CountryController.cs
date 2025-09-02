using CountryService.DataAccess;
using CountryService.DataAccess.Exceptions;
using CountryService.DataAccess.ListQuery;
using CountryService.Mappers.Country;
using CountryService.Models.Country;
using CountryService.Shared;
using CountryService.WebApi.Configuration;
using CountryService.WebApi.ListQuery;
using CountryService.WebApi.Problems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CountryService.WebApi.Controllers;

[Route(Paths.WebApi.Country.BasePath)]
[ApiController]
public class CountryController(
    ILogger<CountryController> logger,
    IDbConnectionFactory dbConnectionFactory,
    IOptionsMonitor<Config> optionsMonitorConfig,
    ICountryDataAccess countryDataAccess,
    ICountryMapper countryMapper,
    ICountryLookupMapper countryLookupMapper,
    IProblemDetailsCreator problemDetailsCreator,
    IQueryValidator queryValidator,
    IQueryReader queryReader) : ControllerBase
{
    [EndpointSummary("Allowed methods")]
    [EndpointDescription("Allow response header, example value: 'GET,POST'.")]
    [HttpOptions]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public void Options()
    {
        Response.Headers.Allow = $"{HttpMethod.Options}, {HttpMethod.Head}, {HttpMethod.Get}, {HttpMethod.Post}, {HttpMethod.Put}, {HttpMethod.Patch}, {HttpMethod.Delete}";
        Response.ContentLength = 0;
    }

    // Example
    // /country?offset=0&limit=10&filters=name:like:unite&filters=isonumber:l:1000&sorts=iso2:desc&sorts=name:asc

    /// <summary>
    /// Paged list of countries.
    /// </summary>
    [EndpointSummary("Get countries")]
    [EndpointDescription("Paged list of countries.")]
    [HttpHead]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<Dtos.Country.Country>>> SelectCountriesAsync(
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

        List<Dtos.Country.Country> countryDtos = countryMapper.MapModelListToDtoList(queryCountriesResult.Item2);

        Response.Headers[AdditionalHeaderNames.Total] = queryCountriesResult.Item1.ToString();

        if (method == HttpMethod.Head.Method)
        {
            Response.Headers.ContentType = Application.Json;
            Response.Headers.ContentLength = countryDtos.ToString()!.Length;

            return new EmptyResult();
        }
        else
        {
            return Ok(countryDtos);
        }
    }

    [EndpointSummary("Get country lookups")]
    [EndpointDescription("List of country lookups with iso2 and name.")]
    [HttpHead(Paths.WebApi.Country.Lookup)]
    [HttpGet(Paths.WebApi.Country.Lookup)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Dtos.Country.CountryLookup>>> GetCountryLookupsAsync()
    {
        string method = HttpContext.Request.Method;

        logger.LogDebug($"GetCountryLookupsAsync, method: {method}");

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        List<CountryLookup> countryLookups = await countryDataAccess.SelectCountryLookupsAsync(dbConnection);

        List<Dtos.Country.CountryLookup> countryLookupDtos = countryLookupMapper.MapModelListToDtoList(countryLookups);

        Response.Headers[AdditionalHeaderNames.Count] = countryLookupDtos.Count.ToString();

        if (method == HttpMethod.Head.Method)
        {
            Response.Headers.ContentType = Application.Json;
            Response.Headers.ContentLength = countryLookups.ToString()!.Length;

            return new EmptyResult();
        }
        else
        {
            return Ok(countryLookupDtos);
        }
    }

    [EndpointSummary("Get country")]
    [EndpointDescription("Get country by iso2.")]
    [HttpHead("{iso2}")]
    [HttpGet("{iso2}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Dtos.Country.Country>> GetCountryByIso2Async(string iso2)
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

            Dtos.Country.Country countryDto = countryMapper.MapModelToDto(country);

            if (method == HttpMethod.Head.Method)
            {
                Response.Headers.ContentType = Application.Json;
                Response.Headers.ContentLength = countryDto.ToString()!.Length;

                return new EmptyResult();
            }
            else
            {
                return Ok(countryDto);
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
                            Shared.Problems.ProblemType.CountryNotFound,
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

    [EndpointSummary("Post country")]
    [EndpointDescription("Add a new country to the database.")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Dtos.Country.Country>> PostCountryAsync([FromBody] Dtos.Country.Country countryDto)
    {
        logger.LogDebug($"PostCountryAsync, countryDto.Iso2: {countryDto.Iso2}");

        Country country = new()
        {
            Iso2 = countryDto.Iso2!,
            Iso3 = countryDto.Iso3!,
            IsoNumber = countryDto.IsoNumber!.Value,
            Name = countryDto.Name!,
            CallingCode = countryDto.CallingCode
        };

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        DbTransaction dbTransaction = await dbConnection.BeginTransactionAsync();

        try
        {
            await countryDataAccess.InsertCountryAsync(country, dbConnection, dbTransaction);

            Country newCountry = await countryDataAccess.SelectCountryByIso2Async(country.Iso2, dbConnection, dbTransaction);

            await dbTransaction.CommitAsync();

            Dtos.Country.Country newCountryDto = countryMapper.MapModelToDto(newCountry);

            return Created($"{optionsMonitorConfig.CurrentValue.WebApiUrl}/country/{country.Iso2}", newCountryDto);
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
                            Shared.Problems.ProblemType.CountryIso2Duplicated,
                            ProblemTitle.CountryIso2Duplicated,
                            $"Country has iso2: '{country.Iso2}' duplicated."));

                case CountryIso3DuplicatedException countryIso3DuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            Shared.Problems.ProblemType.CountryIso3Duplicated,
                            ProblemTitle.CountryIso3Duplicated,
                            $"Country has iso3: '{country.Iso3}' duplicated."));

                case CountryIsoNumberDuplicatedException countryIsoNumberDuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            Shared.Problems.ProblemType.CountryIsoNumberDuplicated,
                            ProblemTitle.CountryIsoNumberDuplicated,
                            $"Country has isoNumber: '{country.IsoNumber}' duplicated."));

                case CountryNameDuplicatedException countryNameDuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            Shared.Problems.ProblemType.CountryNameDuplicated,
                            ProblemTitle.CountryNameDuplicated,
                            $"Country has name: '{country.Name}' duplicated."));

                default:
                    throw;
            }
        }
    }

    [EndpointSummary("Put country")]
    [EndpointDescription("Update an existing country in the database.")]
    [HttpPut("{iso2}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PutCountryByIso2Async(string iso2, [FromBody] Dtos.Country.Country countryDto)
    {
        logger.LogDebug($"PutCountryByIso2Async, iso2: {iso2}");

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        DbTransaction dbTransaction = await dbConnection.BeginTransactionAsync();

        try
        {
            // This will throw if row doesn't exist.
            Country country = await countryDataAccess.SelectCountryByIso2Async(iso2, dbConnection, dbTransaction);

            countryMapper.UpdateModelWithDto(country, countryDto);

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
                            Shared.Problems.ProblemType.CountryNotFound,
                            ProblemTitle.CountryNotFound,
                            $"Country with iso2: '{iso2}' not found."));

                case CountryIso2DuplicatedException countryIso2DuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            Shared.Problems.ProblemType.CountryIso2Duplicated,
                            ProblemTitle.CountryIso2Duplicated,
                            $"Country has iso2: '{countryDto.Iso2}' duplicated."));

                case CountryIso3DuplicatedException countryIso3DuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            Shared.Problems.ProblemType.CountryIso3Duplicated,
                            ProblemTitle.CountryIso3Duplicated,
                            $"Country has iso3: '{countryDto.Iso3}' duplicated."));

                case CountryIsoNumberDuplicatedException countryIsoNumberDuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            Shared.Problems.ProblemType.CountryIsoNumberDuplicated,
                            ProblemTitle.CountryIsoNumberDuplicated,
                            $"Country has isoNumber: '{countryDto.IsoNumber}' duplicated."));

                case CountryNameDuplicatedException countryNameDuplicatedException:
                    return BadRequest(
                        problemDetailsCreator.CreateProblemDetails(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            Shared.Problems.ProblemType.CountryNameDuplicated,
                            ProblemTitle.CountryNameDuplicated,
                            $"Country has name: '{countryDto.Name}' duplicated."));

                default:
                    throw;
            }
        }
    }

    [EndpointSummary("Patch country")]
    [EndpointDescription("Partially update an existing country in the database.")]
    [HttpPatch("{iso2}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PatchCountryByIso2Async([FromRoute] string iso2, [FromBody] JsonPatchDocument<Dtos.Country.Country> countryDtoPatch)
    {
        logger.LogDebug($"PatchCountryByIso2Async, iso2: {iso2}");

        // Currently only supports replace operations.
        if (countryDtoPatch.Operations.Any(x => x.OperationType != Microsoft.AspNetCore.JsonPatch.Operations.OperationType.Replace))
        {
            return BadRequest(
                problemDetailsCreator.CreateProblemDetails(
                    HttpContext,
                    StatusCodes.Status400BadRequest,
                    Shared.Problems.ProblemType.UnsupportedPatchOperation,
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

            Dtos.Country.Country countryDto = countryMapper.MapModelToDto(country);

            // Apply the patch.
            countryDtoPatch.ApplyTo(countryDto, ModelState);
            
            if (!ModelState.IsValid)
            {
                return BadRequest(problemDetailsCreator.CreateValidationProblemDetails(HttpContext, ModelState));
            }

            List<string> dirtyColumns = countryDtoPatch.Operations.Select(x => x.path).ToList();

            logger.LogDebug($"PatchByIso2Async, dirtyColumns: {dirtyColumns}");

            countryMapper.UpdateModelWithDto(country, countryDto);

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
                                Shared.Problems.ProblemType.CountryIso2Duplicated,
                                ProblemTitle.CountryIso2Duplicated,
                                $"Country has iso2: '{country.Iso2}' duplicated."));

                    case CountryIso3DuplicatedException countryIso3DuplicatedException:
                        return BadRequest(
                            problemDetailsCreator.CreateProblemDetails(
                                HttpContext,
                                StatusCodes.Status400BadRequest,
                                Shared.Problems.ProblemType.CountryIso3Duplicated,
                                ProblemTitle.CountryIso3Duplicated,
                                $"Country has iso3: '{country.Iso3}' duplicated."));

                    case CountryIsoNumberDuplicatedException countryIsoNumberDuplicatedException:
                        return BadRequest(
                            problemDetailsCreator.CreateProblemDetails(
                                HttpContext,
                                StatusCodes.Status400BadRequest,
                                Shared.Problems.ProblemType.CountryIsoNumberDuplicated,
                                ProblemTitle.CountryIsoNumberDuplicated,
                                $"Country has isoNumber: '{country.IsoNumber}' duplicated."));

                    case CountryNameDuplicatedException countryNameDuplicatedException:
                        return BadRequest(
                            problemDetailsCreator.CreateProblemDetails(
                                HttpContext,
                                StatusCodes.Status400BadRequest,
                                Shared.Problems.ProblemType.CountryNameDuplicated,
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
                            Shared.Problems.ProblemType.CountryNotFound,
                            ProblemTitle.CountryNotFound,
                            $"Country with iso2: '{iso2}' not found."));

                default:
                    throw;
            }
        }
    }

    [EndpointSummary("Delete country")]
    [EndpointDescription("Remove a country from the database.")]
    [HttpDelete("{iso2}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteCountryByIso2Async(string iso2)
    {
        logger.LogDebug($"DeleteCountryByIso2Async, iso2: {iso2}.");

        using DbConnection dbConnection = dbConnectionFactory.CreateDbConnection();
        await dbConnection.OpenAsync();

        await countryDataAccess.DeleteCountryByIso2Async(iso2, dbConnection);

        return NoContent();
    }

    [EndpointSummary("Does country name exist?")]
    [EndpointDescription("Check whether a country exists with the specified name. If an iso2 value is included then a row with the iso2 is ignored.")]
    [HttpGet]
    [Route(Paths.WebApi.Country.DoesCountryNameExist + "/{name}/{iso2}")]
    [Route(Paths.WebApi.Country.DoesCountryNameExist + "/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
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
