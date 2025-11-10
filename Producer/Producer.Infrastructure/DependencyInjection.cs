using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Producer.Business.Services.Interface.Generators;
using Producer.Common.Config;
using Producer.Infrastructure.Generators;

namespace Producer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddOptions<ProducerOptions>()
            .Bind(cfg.GetSection("Producer"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Paths.BaseFolder), "Producer:Paths:BaseFolder is required")
            .Validate(o => o.RotationPolicy.MaxRecords > 0, "Producer:RotationPolicy:MaxRecords must be > 0")
            .Validate(o => o.WindowSize.ExponentCap >= 0, "Producer:WindowSize:ExponentCap must be >= 0")
            .ValidateOnStart();

        services.AddOptions<PathOptions>().Bind(cfg.GetSection("Producer:Paths")).ValidateOnStart();
        services.AddOptions<RotationPolicyOptions>().Bind(cfg.GetSection("Producer:RotationPolicy")).ValidateOnStart();
        services.AddOptions<IoOptions>().Bind(cfg.GetSection("Producer:IO")).ValidateOnStart();
        services.AddOptions<WindowSizeOptions>().Bind(cfg.GetSection("Producer:WindowSize")).ValidateOnStart();
        services.AddOptions<FaultInjectionOptions>().Bind(cfg.GetSection("Producer:FaultInjection")).ValidateOnStart();
        services.AddOptions<GeneratorTimeOptions>()
            .Bind(cfg.GetSection("Producer:Generator"))
            .Validate(o => o.DayStartTime < o.MiddayTime,
                "Producer:Generator:DayStartTime must be before MiddayTime.")
            .ValidateOnStart();

        services.AddSingleton<IDeliveryStatusGenerator, DeliveryStatusGenerator>();
        services.AddSingleton<IDeliveryListGenerator, DeliveryListGenerator>();
        services.AddSingleton<IFuelPctGenerator, FuelPctGenerator>();
        services.AddSingleton<IOdometerGenerator, OdometerGenerator>();
        services.AddSingleton<ITimestampGenerator, TimestampGenerator>();
        services.AddSingleton<IVehicleIdGenerator, VehicleIdGenerator>();
        services.AddSingleton<IWhatWasAddedGenerator, WhatWasAddedGenerator>();

        return services;
    }
}