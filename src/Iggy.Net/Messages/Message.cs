namespace Iggy.Net.Messages;

public struct Message
{
    public Guid Id { get; set; }
    public byte[] Payload { get; set; }
}

public sealed class HttpMessage
{
    public required UInt128 Id { get; set; }
    public required string Payload { get; set; }
}