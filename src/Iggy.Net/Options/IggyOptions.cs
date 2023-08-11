using Iggy.Net.Enums;
using Iggy.Net.Stream;
using Iggy.Net.Topic;

namespace Iggy.Net.Options;

public sealed class IggyOptions : IIgyOptions
{
    public string BaseAdress { get; set; } = "http://127.0.0.1:3000";
    public Protocol Protocol { get; set; } = Protocol.Http;
    public int ReceiveBufferSize { get; set; } = 8192;
    public int SendBufferSize { get; set; } = 8192;
}

public sealed class StreamOptions
{
    public StreamOptions()
    {
        var baseValue = StreamRegistrar.Options.Count + 1;
        StreamId = baseValue;
        Name = $"stream-{baseValue}";
    }
    public int StreamId { get; set; } = 1;
    public string Name { get; set; } = "stream";
}

public sealed class TopicOptions
{
    public TopicOptions()
    {
        var baseValue = TopicRegistrar.Options.Values.Count + 1;
        TopicId = baseValue;
        Name = $"topic-{baseValue}";
    }
    public int StreamId { get; set; } = 1;
    public string Name { get; set; } = "topic";
    public int TopicId { get; set; } = 1;
    public int PartitionsCount { get; set; } = 3;
}