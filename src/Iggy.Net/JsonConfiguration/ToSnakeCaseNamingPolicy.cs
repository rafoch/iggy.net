using System.Buffers.Binary;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Iggy.Net.Contracts;
using Iggy.Net.Enums;
using Iggy.Net.Extensions;
using Iggy.Net.Messages;

namespace Iggy.Net.JsonConfiguration;

public sealed class ToSnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToSnakeCase();
}

internal sealed class UInt128Converter : JsonConverter<UInt128>
{
    public override UInt128 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException();
        }

        return UInt128.Parse(Encoding.UTF8.GetString(reader.ValueSpan));
    }

    public override void Write(Utf8JsonWriter writer, UInt128 value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToString());
    }
}

internal sealed class MessagesConverter : JsonConverter<MessageSendRequest>
{
    public override MessageSendRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, MessageSendRequest value, JsonSerializerOptions options)
    {
        if (value.Messages.Any())
        {
            var msgList = new List<HttpMessage>();
            foreach (var message in value.Messages)
            {
                var base64 = Convert.ToBase64String(message.Payload);
                msgList.Add(new HttpMessage
                {
                    Id = message.Id.ToUInt128(),
                    Payload = base64
                });
            }

            writer.WriteStartObject();
            writer.WriteStartObject("partitioning");
			
            writer.WriteString(nameof(MessageSendRequest.Partitioning.Kind).ToSnakeCase(), value: value.Partitioning.Kind switch
            {
                PartitioningKind.Balanced => "none",
                PartitioningKind.MessageKey => "entity_id",
                PartitioningKind.PartitionId => "partition_id",
                _ => throw new InvalidEnumArgumentException()
            });
            writer.WriteBase64String(nameof(MessageSendRequest.Partitioning.Value).ToSnakeCase(), value.Partitioning.Value);
            writer.WriteEndObject();
			
            writer.WriteStartArray("messages");
            foreach (var msg in msgList)
            {
                JsonSerializer.Serialize(writer, msg, options);
            }
            writer.WriteEndArray();
			
            writer.WriteEndObject();
            return;
        }
        writer.WriteStringValue("");
    }
}

internal static class UInt128Extensions
{
    internal static UInt128 ToUInt128(this Guid g)
    {
        Span<byte> array = stackalloc byte[16];
        MemoryMarshal.TryWrite(array, ref g);
        var hi = BinaryPrimitives.ReadUInt64LittleEndian(array[0..8]);
        var lo = BinaryPrimitives.ReadUInt64LittleEndian(array[8..16]);
        return new UInt128(hi, lo);
    }
    
    internal static byte[] GetBytesFromUInt128(this UInt128 value)
    {

        Span<byte> result = stackalloc byte[16];
        var span = MemoryMarshal.Cast<UInt128, byte>(MemoryMarshal.CreateReadOnlySpan(ref value, 1));
        for (int i = 0; i < 16; i++)
        {
            result[i] = span[i];
        }
        return result.ToArray();
    }
    
    internal static UInt128 GetUInt128(this JsonElement jsonElement)
    {
        return UInt128.Parse(jsonElement.ToString());
    }

    private static int CountUppercaseLetters(string input)
    {
        return input.Count(char.IsUpper);
    }
    private static void ShiftSliceRight(this Span<char> slice)
    {
        for (int i = slice.Length - 2; i >= 0; i--)
        {
            slice[i + 1] = slice[i];
        }
    }
}

public sealed class MessageResponseGenericConverter<TMessage> : JsonConverter<IEnumerable<MessageResponse<TMessage>>>
{
    private readonly Func<byte[], TMessage> _serializer;

    public MessageResponseGenericConverter(Func<byte[], TMessage> serializer)
    {
        _serializer = serializer;
    }
    public override IEnumerable<MessageResponse<TMessage>>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //TODO - mby get rid of this allocation as well 
        var messageResponses = new List<MessageResponse<TMessage>>();
        using var doc = JsonDocument.ParseValue(ref reader);
		
        var root = doc.RootElement;
        foreach (var element in root.EnumerateArray())
        {
            var offset = element.GetProperty(nameof(MessageResponse.Offset).ToSnakeCase()).GetUInt64();
            var timestamp = element.GetProperty(nameof(MessageResponse.Timestamp).ToSnakeCase()).GetUInt64();
            var id = element.GetProperty(nameof(MessageResponse.Id).ToSnakeCase()).GetUInt128();
            var payload = element.GetProperty(nameof(MessageResponse.Payload).ToSnakeCase()).GetBytesFromBase64();

            messageResponses.Add(new MessageResponse<TMessage>
            {
                Offset = offset,
                Timestamp = timestamp,
                Id = new Guid(id.GetBytesFromUInt128()), 
                Message = _serializer(payload)
            });
        }

        return messageResponses;
    }

    public override void Write(Utf8JsonWriter writer, IEnumerable<MessageResponse<TMessage>> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

internal sealed class MessageResponseConverter : JsonConverter<IEnumerable<MessageResponse>>
{
    public override IEnumerable<MessageResponse> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //TODO - maby get rid of this allocation by using MemoryPools
        var messageResponses = new List<MessageResponse>();
        using var doc = JsonDocument.ParseValue(ref reader);
		
        var root = doc.RootElement;
        foreach (var element in root.EnumerateArray())
        {
            var offset = element.GetProperty(nameof(MessageResponse.Offset).ToSnakeCase()).GetUInt64();
            var timestamp = element.GetProperty(nameof(MessageResponse.Timestamp).ToSnakeCase()).GetUInt64();
            var id = element.GetProperty(nameof(MessageResponse.Id).ToSnakeCase()).GetUInt128();
            var payload = element.GetProperty(nameof(MessageResponse.Payload).ToSnakeCase()).GetBytesFromBase64();

            messageResponses.Add(new MessageResponse
            {
                Offset = offset,
                Timestamp = timestamp,
                Id = new Guid(id.GetBytesFromUInt128()), 
                Payload = payload
            });
        }

        return messageResponses;
    }

    public override void Write(Utf8JsonWriter writer, IEnumerable<MessageResponse> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}