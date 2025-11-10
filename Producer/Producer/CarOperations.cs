using Microsoft.Extensions.DependencyInjection;
using Producer.Business.Mappers.CarMapper;
using Producer.Business.Services.Interface;
using Producer.Common;

namespace Producer;

public class CarOperations(
    IFileSystemService writer,
    ICarTelemetryService telemetry,
    IServiceProvider services,
    IVehicleContext ctx)
{
    private readonly ICarMapper _mapper = services.GetRequiredKeyedService<ICarMapper>(ctx.Version);

    public async Task PublishCarData()
    {
        var result = telemetry.GenerateValue();
        var dto = _mapper.Map(result);
        await writer.AddAsync(dto);
    }

    public void FinalizeWriting()
    {
        writer.FinalizeOnShutdownSync();
    }
}