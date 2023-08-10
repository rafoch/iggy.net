using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Iggy.Net.Extensions;

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