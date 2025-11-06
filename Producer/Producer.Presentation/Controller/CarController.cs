using Microsoft.Extensions.DependencyInjection;
using Producer.Business.Entity;
using Producer.Business.Mappers;
using Producer.Business.Services.Implementation;
using Producer.Business.Services.Interface;
using Producer.Common;

namespace Producer.Presentation.Controller;

public class CarController(
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