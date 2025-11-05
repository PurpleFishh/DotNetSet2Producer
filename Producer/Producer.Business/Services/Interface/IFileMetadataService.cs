using Producer.Business.Entity;

namespace Producer.Business.Services.Interface;

public interface IFileMetadataService
{
    public Task<MetaData> GetFileMetaData(string filePath, string version, int recordCount,
        CompressionKind compression);

    public Task WriteMetaDataFile(string filePath, string version, int recordCount,
        CompressionKind compression);

    public Task WriteMetaDataFileForFinal(string finalPath, string version, int recordCount,
        CompressionKind compression, string sha256AlreadyComputed);
}