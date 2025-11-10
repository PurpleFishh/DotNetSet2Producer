using System.Text.Json;
using System.Text.Json.Serialization;

namespace Producer.Common.Types;

[JsonConverter(typeof(CompressionKindConverter))]
public enum FileCompressionType
{
    None,
    Gzip
}

public sealed class CompressionKindConverter : JsonConverter<FileCompressionType>
{
    public override FileCompressionType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string token for compression kind");
        var value = reader.GetString();
        return value switch
        {
            "gzip" => FileCompressionType.Gzip,
            "none" => FileCompressionType.None,
            _ => throw new JsonException($"Unknown compression kind '{value}'")
        };
    }

    public override void Write(Utf8JsonWriter writer, FileCompressionType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value == FileCompressionType.Gzip ? "gzip" : "none");
    }
}