using Producer.Entity;

namespace Producer.Dto;

public record CarDtoV2(
    string VehicleId,
    DateTime TsUtc,
    double Odometer,
    double FuelPct,
    string DeliveryStatus,
    List<int> DeliveryList,
    List<int>? WhatWasAdded,
    string SchemaVersion = "2.0"
);