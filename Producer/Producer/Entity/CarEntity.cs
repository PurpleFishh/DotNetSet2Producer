namespace Producer.Entity;

public record CarEntity(
    string VehicleId,
    DateTime TsUtc,
    double Odometer,
    double FuelPct,
    DeliveryStatus DeliveryStatus,
    List<int> DeliveryList,
    List<int>? WhatWasAdded
);

public enum DeliveryStatus
{
    PickUp,
    InProgress,
    Completed
}