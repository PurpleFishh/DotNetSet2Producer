using Generator;
using Generator.Generators;
using Generator.Generators.Helper;
using Generator.Random;
using Producer.Entity;
using Producer.Generators;
using Producer.Mappers;

namespace Producer.Services;

public class CarTelemetryService
{
    private readonly IRandomSource _rnd;
    private readonly GenerationContext _ctx;
    private readonly string _vehicleId;

    private readonly RecordBuilder<CarEntity> _builder;

    public CarTelemetryService(
        string vehicleId,
        // (double lat, double lon) center,
        int? seed
    )
    {
        _vehicleId = vehicleId;
        _rnd = new DefaultRandomSource(seed);
        _ctx = new GenerationContext();

        var engineOn = EngineStatusGenerator.Get();
        var speed = SpeedGenerator.Get();
        var fuelPct = FuelPctGenerator.Get();
        var coolant = CoolantInfoGenerator.Get();
        var gps = GpsGenerator.Get();
        var odo = OdoInfoGenerator.Get();

        var predefined = new Dictionary<string, Func<object>>
        {
            ["VehicleId"] = () => vehicleId,
            ["TsUtc"] = () => DateTime.UtcNow
        };

        CarEntity DtoMapper(IReadOnlyDictionary<string, object> d)
            => GeneratedInfoMapper.MapTo<CarEntity>(d, predefined);

        _builder = new RecordBuilder<CarEntity>(DtoMapper)
            .AddStep("EngineOn", engineOn)
            .AddStep("SpeedKmh", new Round<double>(speed, 1))
            .AddStep("FuelPct", new Round<double>(fuelPct, 1))
            .AddStep("CoolantTempC", new Round<double>(coolant, 1))
            .AddStep("Gps", gps)
            .AddStep("OdoKm", odo);
    }

    public CarEntity Next() => _builder.Build(_rnd, _ctx);
}