using System.Text.Json;
using System.Text.Json.Serialization;

namespace Producer.Business.Entity;

public record MetaData
{
    [JsonPropertyName("version")] public string Version { get; init; } = string.Empty;

    [JsonPropertyName("createdUtc")] public string CreatedUtc { get; init; } = string.Empty;

    [JsonPropertyName("recordCount")] public int RecordCount { get; init; }

    [JsonPropertyName("sha256")] public string Sha256 { get; init; } = string.Empty;

    [JsonPropertyName("encoding")] public string Encoding { get; init; } = string.Empty;

    [JsonPropertyName("compression")] public CompressionKind Compression { get; init; } = CompressionKind.None;
}

[JsonConverter(typeof(CompressionKindConverter))]
public enum CompressionKind
{
    None,
    Gzip
}

public sealed class CompressionKindConverter : JsonConverter<CompressionKind>
{
    public override CompressionKind Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string token for compression kind");
        var value = reader.GetString();
        return value switch
        {
            "gzip" => CompressionKind.Gzip,
            "none" => CompressionKind.None,
            _ => throw new JsonException($"Unknown compression kind '{value}'")
        };
    }

    public override void Write(Utf8JsonWriter writer, CompressionKind value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value == CompressionKind.Gzip ? "gzip" : "none");
    }
}