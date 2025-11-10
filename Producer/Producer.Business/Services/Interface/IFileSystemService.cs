namespace Producer.Business.Services.Interface;

public interface IFileSystemService : IDisposable, IAsyncDisposable
{
    public Task AddAsync(object record);
    public Task FinalizeOnShutdownAsync();
    public void FinalizeOnShutdownSync();
}