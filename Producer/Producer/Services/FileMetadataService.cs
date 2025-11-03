using System.Text.Json;
using Producer.Entity;

namespace Producer.Services;

public class FileMetadataService
{
    private const string Encoding = "utf8";
    private const string MetadataFileExtension = ".meta.json";

    public async Task<MetaData> GetFileMetaData(string filePath, string version, int recordCount,
        CompressionKind compression)
    {
        var sha256 = await new FileChecksum().GetChecksum(filePath);
        var meta = new MetaData
        {
            Version = version,
            CreatedUtc = DateTime.UtcNow.ToString("o"),
            RecordCount = recordCount,
            Sha256 = sha256,
            Encoding = Encoding,
            Compression = compression
        };
        return meta;
    }

    public async Task WriteMetaDataFile(string filePath, string version, int recordCount,
        CompressionKind compression)
    {
        var meta = await GetFileMetaData(filePath, version, recordCount, compression);

        var metaPath = filePath + MetadataFileExtension;
        await File.WriteAllTextAsync(metaPath,
            JsonSerializer.Serialize(meta, new JsonSerializerOptions { WriteIndented = true }));
    }

    public Task WriteMetaDataFileForFinal(string finalPath, string version, int recordCount,
        CompressionKind compression, string sha256AlreadyComputed)
    {
        var meta = new MetaData
        {
            Version = version,
            CreatedUtc = DateTime.UtcNow.ToString("o"),
            RecordCount = recordCount,
            Sha256 = sha256AlreadyComputed,
            Encoding = Encoding,
            Compression = compression
        };

        var metaPath = finalPath + MetadataFileExtension;
        var json = JsonSerializer.Serialize(meta, new JsonSerializerOptions { WriteIndented = true });
        return File.WriteAllTextAsync(metaPath, json);
    }
}