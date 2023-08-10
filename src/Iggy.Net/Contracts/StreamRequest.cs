namespace Iggy.Net.Contracts;

public sealed class StreamRequest
{
    public required int StreamId { get; init; }
    public required string Name { get; init; }
}