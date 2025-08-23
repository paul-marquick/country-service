using System.Linq;
using System.Net.Http;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CountryService.Shared;
using Microsoft.AspNetCore.Mvc;

namespace CountryService.BlazorAdminApp.HttpClients;

public static class HttpResponseMessageExtensions
{
    public static string GetAllowedMethods(this HttpResponseMessage response)
    {
        return response.Headers.GetValues(HeaderNames.Allow).First();
    }

    public static int GetCount(this HttpResponseMessage response)
    {
        return int.Parse(response.Headers.GetValues(AdditionalHeaderNames.Count).First());
    }

    public static long GetTotal(this HttpResponseMessage response)
    {
        return long.Parse(response.Headers.GetValues(AdditionalHeaderNames.Total).First());
    }

    public static async Task<T> GetJsonDataAsync<T>(this HttpResponseMessage response)
    {
        return await DeserializeJsonAsync<T>(response);
    }

    public static async Task CheckStatusAsync(this HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw await CreateValidationProblemDetailsExceptionAsync(response);
        }
    }

    private static async Task<ValidationProblemDetailsException> CreateValidationProblemDetailsExceptionAsync(HttpResponseMessage response)
    {
        return new ValidationProblemDetailsException(await DeserializeJsonAsync<ValidationProblemDetails>(response));
    }

    /// <summary>
    /// Converts json into an object.
    /// </summary>
    private static async Task<T> DeserializeJsonAsync<T>(HttpResponseMessage response)
    {
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), jsonSerializerOptions)!;
    }
}
