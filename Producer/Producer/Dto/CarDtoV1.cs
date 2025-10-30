namespace Producer.Dto;

public record CarDtoV1(
    string VehicleId,
    DateTime TsUtc,
    double SpeedKmh,
    double FuelPct,
    double CoolantTempC
);