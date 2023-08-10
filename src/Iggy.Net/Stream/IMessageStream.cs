using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Iggy.Net.Contracts;
using Iggy.Net.JsonConfiguration;

namespace Iggy.Net.Stream;

public interface IMessageStream
{
    Task CreateStreamAsync(StreamRequest request);
}

public sealed class HttpMessageStream : IMessageStream
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _toSnakeCaseOptions;

    public HttpMessageStream(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _toSnakeCaseOptions = new();
        
        _toSnakeCaseOptions.PropertyNamingPolicy = new ToSnakeCaseNamingPolicy();
        _toSnakeCaseOptions.WriteIndented = true;
        
        _toSnakeCaseOptions.Converters.Add(new UInt128Converter());
        _toSnakeCaseOptions.Converters.Add(new JsonStringEnumConverter(new ToSnakeCaseNamingPolicy()));
    }

    public async Task CreateStreamAsync(StreamRequest request)
    {
        var json = JsonSerializer.Serialize(request, _toSnakeCaseOptions);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/streams", data);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Exception");
        }
    }
}