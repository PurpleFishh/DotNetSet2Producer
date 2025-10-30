namespace Producer.Entity;

public record CarEntity(
    string VehicleId,
    DateTime TsUtc,
    double SpeedKmh,
    double FuelPct,
    double CoolantTempC,
    GpsInfo Gps,
    bool EngineOn,
    double OdoKm
);

public record GpsInfo(
    double Lat,
    double Lon
);  