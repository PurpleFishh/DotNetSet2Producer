namespace Producer.Business.Services.Interface;

public interface IBackpressureService
{
    public Task ApplyBackpressureAsync();
}