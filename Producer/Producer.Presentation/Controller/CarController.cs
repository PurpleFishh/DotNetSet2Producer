using Producer.Business.Entity;
using Producer.Business.Mappers;
using Producer.Business.Services.Implementation;
using Producer.Business.Services.Interface;

namespace Producer.Presentation.Controller;

public class CarController(string carId, DataSchemas schema, FileSystemService writer)
{
    private readonly ICarTelemetryService _carTelemetryService = new CarTelemetryService(carId, carId.GetHashCode());

    public async Task InfoPublish()
    {
        var result = _carTelemetryService.Next();

        switch (schema)
        {
            case DataSchemas.V1_0: await writer.AddAsync(result.ToDtoV1()); break;
            case DataSchemas.V2_0: await writer.AddAsync(result.ToDtoV2()); break;
        }
    }
}