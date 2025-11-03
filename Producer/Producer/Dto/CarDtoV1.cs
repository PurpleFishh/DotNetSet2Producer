using Producer.Entity;

namespace Producer.Dto;

public record CarDtoV1(
    string VehicleId,
    DateTime TsUtc,
    string DeliveryStatus,
    List<int> DeliveryList,
    List<int>? WhatWasAdded,
    string SchemaVersion = "1.0"
);