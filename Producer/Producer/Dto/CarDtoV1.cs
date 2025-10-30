namespace Producer.Generator;

public record CarDtoV1(
    string vehicleId,
    DateTime tsUtc,
    double speedKmh,
    double fuelPct,
    double coolantTempC,
    (double lat, double lon) gps,
    bool engineOn,
    double odoKm
);