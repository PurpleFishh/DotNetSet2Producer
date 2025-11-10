using System.Text.Json.Serialization;
using Producer.Common.Types;

namespace Producer.Business.Entity;

public record FileMetaDataEntity
{
    [JsonPropertyName("version")] public string Version { get; init; } = string.Empty;

    [JsonPropertyName("createdUtc")] public string CreatedUtc { get; init; } = string.Empty;

    [JsonPropertyName("recordCount")] public int RecordCount { get; init; }

    [JsonPropertyName("sha256")] public string Sha256 { get; init; } = string.Empty;

    [JsonPropertyName("encoding")] public string Encoding { get; init; } = string.Empty;

    [JsonPropertyName("compression")] public FileCompressionType FileCompression { get; init; } = FileCompressionType.None;
}

