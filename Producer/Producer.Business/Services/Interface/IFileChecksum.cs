namespace Producer.Business.Services.Interface;

public interface IFileChecksum
{
    public Task<string> GetChecksum(string filePath);
}