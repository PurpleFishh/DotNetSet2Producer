using Producer.Business.Entity;
using Producer.Common.Types;

namespace Producer.Business.Services.Interface;

public interface IFileMetadataService
{
    public Task<FileMetaDataEntity> GetFileMetaData(string filePath, string version, int recordCount,
        FileCompressionType fileCompression);

    public Task WriteMetaDataFile(string filePath, string version, int recordCount,
        FileCompressionType fileCompression);

    public Task WriteMetaDataFileForFinal(string finalPath, DataSchemas version, int recordCount,
        FileCompressionType fileCompression, string sha256AlreadyComputed);
}