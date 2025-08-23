using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace CountryService.BlazorAdminApp.HttpClients;

public static class HttpRequestMessageExtensions
{
    public static void AddJsonData(this HttpRequestMessage request, object data)
    {
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        request.Content = new StringContent(JsonSerializer.Serialize(data, jsonSerializerOptions), Encoding.UTF8, Application.Json);
    }

    public static void AddFormData(this HttpRequestMessage request, IEnumerable<KeyValuePair<string?, string?>> form)
    {
        request.Content = new FormUrlEncodedContent(form);
    }
}
