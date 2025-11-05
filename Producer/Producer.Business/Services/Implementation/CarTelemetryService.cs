using Generator;
using Generator.Random;
using Producer.Business.Entity;
using Producer.Business.Mappers;
using Producer.Business.Services.Interface;
using Producer.Infrastructure.Generators;

namespace Producer.Business.Services.Implementation;

public class CarTelemetryService : ICarTelemetryService
{
    private readonly IRandomSource _rnd;
    private readonly GenerationContext _ctx;

    private readonly RecordBuilder<CarEntity> _builder;

    public CarTelemetryService(
        string vehicleId,
        int? seed
    )
    {
        _rnd = new DefaultRandomSource(seed);
        _ctx = new GenerationContext();


        var ts = TimestampGenerator.Get(0, 30);
        var vehicleIdGetter = VehicleIdGenerator.Get(vehicleId);
        var deliveryStatus = DeliveryStatusGenerator.Get();
        var deliveryList = DeliveryListGenerator.Get();
        var whatWasAdded = WhatWasAddedGenerator.Get();
        var odometer = OdometerGenerator.Get();
        var fuelPct = FuelPctGenerator.Get();

        _ctx.Set("MiddayTime", GeneratorConstants.MiddayTime);
        _ctx.Set("DayStartTime", GeneratorConstants.DayStartTime);

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
            .AddStep("WhatWasAdded", whatWasAdded)
            .AddStep("DeliveryList", deliveryList)
            .AddStep("DeliveryStatus", deliveryStatus)
            .AddStep("Odometer", odometer)
            .AddStep("FuelPct", fuelPct);
    }

    public CarEntity Next() => _builder.Build(_rnd, _ctx);
}