using Producer.Business.Entity;
using Producer.Common;

namespace Producer.Business.Services.Interface;

public interface IFileMetadataService
{
    public Task<MetaData> GetFileMetaData(string filePath, string version, int recordCount,
        CompressionKind compression);

    public Task WriteMetaDataFile(string filePath, string version, int recordCount,
        CompressionKind compression);

    public Task WriteMetaDataFileForFinal(string finalPath, DataSchemas version, int recordCount,
        CompressionKind compression, string sha256AlreadyComputed);
}