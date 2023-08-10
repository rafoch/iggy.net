using Iggy.Net.Enums;

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
    public int Id { get; set; } = 1;
    public string Name { get; set; } = "stream";
}

public sealed class TopicOptions
{
    public int StreamId { get; set; } = 1;
    public string Name { get; set; } = "topic";
    public int TopicId { get; set; } = 1;
}