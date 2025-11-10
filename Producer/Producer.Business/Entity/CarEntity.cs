using Producer.Common.Types;

namespace Producer.Business.Entity;

public record CarEntity(
    string VehicleId,
    DateTime TsUtc,
    double Odometer,
    double FuelPct,
    DeliveryStatus DeliveryStatus,
    List<int> DeliveryList,
    List<int>? WhatWasAdded
);