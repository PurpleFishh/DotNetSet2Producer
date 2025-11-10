using System.Text.Json;
using Producer.Business.Entity;
using Producer.Business.Services.Interface;
using Producer.Common.Types;

namespace Producer.Business.Services.Implementation;

public class FileMetadataService : IFileMetadataService
{
    private const string Encoding = "utf8";
    private const string MetadataFileExtension = ".meta.json";

    public async Task<FileMetaDataEntity> GetFileMetaData(string filePath, string version, int recordCount,
        FileCompressionType fileCompression)
    {
        var sha256 = await new FileChecksum().GetChecksum(filePath);
        var meta = new FileMetaDataEntity
        {
            Version = version,
            CreatedUtc = DateTime.UtcNow.ToString("o"),
            RecordCount = recordCount,
            Sha256 = sha256,
            Encoding = Encoding,
            FileCompression = fileCompression
        };
        return meta;
    }

    public async Task WriteMetaDataFile(string filePath, string version, int recordCount,
        FileCompressionType fileCompression)
    {
        var meta = await GetFileMetaData(filePath, version, recordCount, fileCompression);

        var metaPath = filePath + MetadataFileExtension;
        await File.WriteAllTextAsync(metaPath,
            JsonSerializer.Serialize(meta, new JsonSerializerOptions { WriteIndented = true }));
    }

    public Task WriteMetaDataFileForFinal(string finalPath, DataSchemas version, int recordCount,
        FileCompressionType fileCompression, string sha256AlreadyComputed)
    {
        var meta = new FileMetaDataEntity
        {
            Version = version.ToString(),
            CreatedUtc = DateTime.UtcNow.ToString("o"),
            RecordCount = recordCount,
            Sha256 = sha256AlreadyComputed,
            Encoding = Encoding,
            FileCompression = fileCompression
        };

        var metaPath = finalPath + MetadataFileExtension;
        var json = JsonSerializer.Serialize(meta, new JsonSerializerOptions { WriteIndented = true });
        return File.WriteAllTextAsync(String.Join("", metaPath.Split(".tmp")), json);
    }
}