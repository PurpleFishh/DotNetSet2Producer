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
        int? seed
    )
    {
        _rnd = new DefaultRandomSource(seed);
        _ctx = new GenerationContext();

        var engineOn = EngineStatusGenerator.Get();

        var ts = TimestampGenerator.Get(0, 30);
        var vehicleIdGetter = VehicleIdGenerator.Get(vehicleId);

        var deliveryStatus = DeliveryStatusGenerator.Get();

        var deliveryList = DeliveryListGenerator.Get();
        var whatWasAdded = WhatWasAddedGenerator.Get();

        var odometer = OdometerGenerator.Get();
        var fuelPct = FuelPctGenerator.Get();

        _ctx.Set("MiddayTime", new TimeOnly(12, 00, 00));

        var predefined = new Dictionary<string, Func<object>>
        {
            // ["VehicleId"] = () => vehicleIdGetter.Next(_rnd, _ctx),
            // ["TsUtc"] = () => ts.Next(_rnd, _ctx)
        };

        CarEntity DtoMapper(IReadOnlyDictionary<string, object> d)
            => GeneratedInfoMapper.MapTo<CarEntity>(d, predefined);

        _builder = new RecordBuilder<CarEntity>(DtoMapper)
            .AddStep("VehicleId", vehicleIdGetter)
            .AddStep("TsUtc", ts)
            .AddStep("DeliveryStatus", deliveryStatus)
            .AddStep("DeliveryList", deliveryList)
            .AddStep("WhatWasAdded", whatWasAdded)
            .AddStep("Odometer", odometer)
            .AddStep("FuelPct", fuelPct);
    }

    public CarEntity Next() => _builder.Build(_rnd, _ctx);
}