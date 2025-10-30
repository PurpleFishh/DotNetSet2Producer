namespace Producer.Dto;

public record CarDtoV2(
    string VehicleId,
    DateTime TsUtc,
    double SpeedKmh,
    double FuelPct,
    double CoolantTempC,
    GpsDto Gps,
    bool EngineOn,
    double OdoKm
);

public record GpsDto(
    double Lat,
    double Lon
);