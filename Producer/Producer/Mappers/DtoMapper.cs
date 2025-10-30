using Producer.Dto;
using Producer.Entity;
using Producer.Services;

namespace Producer.Mappers;

public static class DtoMapper
{
    public static CarDtoV1 ToDtoV1(this CarEntity car) =>
        new CarDtoV1(
            VehicleId: car.VehicleId,
            TsUtc: car.TsUtc,
            SpeedKmh: car.SpeedKmh,
            FuelPct: car.FuelPct,
            CoolantTempC: car.CoolantTempC
        );

    public static CarDtoV2 ToDtoV2(this CarEntity car)
    {
        return new CarDtoV2(
            VehicleId: car.VehicleId,
            TsUtc: car.TsUtc,
            SpeedKmh: car.SpeedKmh,
            FuelPct: car.FuelPct,
            CoolantTempC: car.CoolantTempC,
            Gps: car.Gps.ToGpsDto(),
            EngineOn: car.EngineOn,
            OdoKm: car.OdoKm
        );
    }

    private static GpsDto ToGpsDto(this GpsInfo gps) =>
        new GpsDto(
            Lat: gps.Lat,
            Lon: gps.Lon
        );
}