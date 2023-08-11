using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Iggy.Net.Contracts;
using Iggy.Net.Extensions;
using Iggy.Net.JsonConfiguration;
using Iggy.Net.Options;

namespace Iggy.Net.Stream;

public interface IMessageStream
{
    Task CreateStreamAsync(StreamRequest request);
    Task CreateTopicAsync(int streamId, TopicRequest topic);
    Task SendMessagesAsync(int streamId, int topicId, MessageSendRequest request);
    Task<IAsyncEnumerable<MessageResponse>> PollMessagesAsync(MessageFetchRequest request);
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

    public async Task CreateTopicAsync(int streamId, TopicRequest topic)
    {
        var json = JsonSerializer.Serialize(topic, _toSnakeCaseOptions);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"/streams/{streamId}/topics", data);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Exception");
        }
    }

    public async Task SendMessagesAsync(int streamId, int topicId, MessageSendRequest request)
    {
        var json = JsonSerializer.Serialize(request, _toSnakeCaseOptions);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"/streams/{streamId}/topics/{topicId}/messages", data);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Exception");
        }
    }

    public async Task<IAsyncEnumerable<MessageResponse>> PollMessagesAsync(MessageFetchRequest request)
    {
        var url = CreateUrl($"/streams/{request.StreamId}/topics/{request.TopicId}/messages?consumer_id={request.Consumer.Id}" +
                  $"&partition_id={request.PartitionId}&kind={request.PollingStrategy}&value={request.Value}&count={request.Count}&auto_commit={request.AutoCommit}");

        var response = await _httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IAsyncEnumerable<MessageResponse>>(
                new JsonSerializerOptions()
                {
                    Converters = { new MessageResponseConverter() }
                });
        }

        return null;
    }
    
    private static string CreateUrl(ref MessageRequestInterpolationHandler message)
    {
        return message.ToString();
    }
}

public static class StreamRegistrar
{
    public static ConcurrentBag<StreamOptions> Options = new ConcurrentBag<StreamOptions>();
}