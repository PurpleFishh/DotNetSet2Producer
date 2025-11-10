using Producer.Business.Dto;
using Producer.Business.Entity;

namespace Producer.Business.Mappers;

public static class DtoMapperExtentions
{
    public static CarDtoV1 ToDtoV1(this CarEntity car) =>
        new(
            VehicleId: car.VehicleId,
            TsUtc: car.TsUtc,
            DeliveryList: car.DeliveryList,
            WhatWasAdded: car.WhatWasAdded,
            DeliveryStatus: car.DeliveryStatus.ToString()
        );

    public static CarDtoV2 ToDtoV2(this CarEntity car) =>
        new(
            VehicleId: car.VehicleId,
            TsUtc: car.TsUtc,
            DeliveryList: car.DeliveryList,
            WhatWasAdded: car.WhatWasAdded,
            DeliveryStatus: car.DeliveryStatus.ToString(),
            Odometer: car.Odometer,
            FuelPct: car.FuelPct
        );
}