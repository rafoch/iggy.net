namespace Iggy.Net.Enums;

public enum Protocol
{
    Http,
    Tcp,
    Quic
}

public enum ConsumerType
{
    Consumer,
    ConsumerGroup
}

public enum IdKind
{
    Numeric,
    String
}

public enum MessagePolling
{
    Offset,
    Timestamp,
    First,
    Last,
    Next
}

public enum PartitioningKind
{
    Balanced,
    PartitionId,
    MessageKey
}