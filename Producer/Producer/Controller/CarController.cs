using Producer.Entity;
using Producer.Mappers;
using Producer.Services;

namespace Producer.Controller;

public class CarController(string carId)
{
    private readonly CarTelemetryService _carTelemetryService = new(carId, carId.GetHashCode());

    public async Task InfoPublish()
    {
        var writer = new FileSystemService("inbox", carId, compress: CompressionKind.None);

        for (var i = 0; i < 12000; i++)
        {
            var result = _carTelemetryService.Next();
            await writer.AddAsync(result.ToDtoV2());
            await Task.Delay(10);
        }
    }
}