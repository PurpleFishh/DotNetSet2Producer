namespace Producer.Business.Dto;

public record CarDtoV1(
    string VehicleId,
    DateTime TsUtc,
    string DeliveryStatus,
    List<int> DeliveryList,
    List<int>? WhatWasAdded,
    string SchemaVersion = "v1"
);