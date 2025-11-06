using Generator;
using Generator.Generators;
using Generator.Random;
using Producer.Business.Entity;
using Producer.Business.Mappers;
using Producer.Business.Services.Interface;
using Producer.Business.Services.Interface.Generators;
using Producer.Common;

namespace Producer.Business.Services.Implementation;

public class CarDataGeneratorService(
    IVehicleContext vehicle,
    ITimestampGenerator timestampGenerator,
    IVehicleIdGenerator vehicleIdGenerator,
    IDeliveryStatusGenerator deliveryStatusGenerator,
    IDeliveryListGenerator deliveryListGenerator,
    IWhatWasAddedGenerator whatWasAddedGenerator,
    IOdometerGenerator odometerGenerator,
    IFuelPctGenerator fuelPctGenerator) : IDataGeneratorService<CarEntity>
{
    private readonly IValueGenerator<DateTime> _ts = timestampGenerator.Get(0, 30);
    private readonly IValueGenerator<string> _vehicleIdGetter = vehicleIdGenerator.Get(vehicle.VehicleId);
    private readonly IValueGenerator<DeliveryStatus> _deliveryStatus = deliveryStatusGenerator.Get();
    private readonly IValueGenerator<List<int>> _deliveryList = deliveryListGenerator.Get();
    private readonly IValueGenerator<List<int>?> _whatWasAdded = whatWasAddedGenerator.Get();
    private readonly IValueGenerator<double> _odometer = odometerGenerator.Get();
    private readonly IValueGenerator<double> _fuelPct = fuelPctGenerator.Get();

    public RecordBuilder<CarEntity> GetGenerator()
    {
        var predefined = new Dictionary<string, Func<object>>();

        CarEntity DtoMapper(IReadOnlyDictionary<string, object> generatedValues)
            => GeneratedInfoMapper.MapTo<CarEntity>(generatedValues, predefined);

        return new RecordBuilder<CarEntity>(DtoMapper)
            .AddStep(nameof(CarEntity.VehicleId), _vehicleIdGetter)
            .AddStep(nameof(CarEntity.TsUtc), _ts)
            .AddStep(nameof(CarEntity.WhatWasAdded), _whatWasAdded)
            .AddStep(nameof(CarEntity.DeliveryList), _deliveryList)
            .AddStep(nameof(CarEntity.DeliveryStatus), _deliveryStatus)
            .AddStep(nameof(CarEntity.Odometer), _odometer)
            .AddStep(nameof(CarEntity.FuelPct), _fuelPct);
    }
}