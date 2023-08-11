using System.Text.Json.Serialization;
using Iggy.Net.Enums;
using Iggy.Net.JsonConfiguration;
using Iggy.Net.Kinds;
using Iggy.Net.Messages;

namespace Iggy.Net.Contracts;

public sealed class TopicRequest
{
    public required int TopicId { get; init; }
    public required string Name { get; init; }
    public required int PartitionsCount{ get; init; }
}

[JsonConverter(typeof(MessagesConverter))]
public sealed class MessageSendRequest
{
    public required Partitioning Partitioning { get; init; }
    public required ICollection<Message> Messages { get; init; }
}

public sealed class MessageFetchRequest
{
    public required Consumer Consumer { get; init; }
    public required int StreamId { get; init; }	
    public required int TopicId { get; init; }
    public required int PartitionId { get; init; }
    public required MessagePolling PollingStrategy { get; init; }
    public required ulong Value { get; init; }
    public required int Count { get; init; }
    public required bool AutoCommit { get; init; }
}

public sealed class Consumer
{
    public required ConsumerType Type { get; init; }
    public required int Id { get; init; }

    public static Consumer New(int id)
    {
        return new Consumer
        {
            Id = id,
            Type = ConsumerType.Consumer
        };
    }

    public static Consumer Group(int id)
    {
        return new Consumer
        {
            Id = id,
            Type = ConsumerType.ConsumerGroup
        };
    }
}

public sealed class MessageResponse
{
    public required ulong Offset { get; init; }
    public required ulong Timestamp { get; init; }
    public Guid Id { get; init; }
    public required byte[] Payload { get; init; }
}

public sealed class MessageResponse<T>
{
    public required ulong Offset { get; init; }
    public required ulong Timestamp { get; init; }
    public Guid Id { get; init; }
    public required T Message { get; init; }
}