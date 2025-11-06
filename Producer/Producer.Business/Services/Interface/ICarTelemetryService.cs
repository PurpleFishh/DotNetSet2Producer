using Producer.Business.Entity;

namespace Producer.Business.Services.Interface;

public interface ICarTelemetryService
{
    public CarEntity GenerateValue();
}