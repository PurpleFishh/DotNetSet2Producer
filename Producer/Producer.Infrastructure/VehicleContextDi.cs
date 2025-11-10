using Microsoft.Extensions.DependencyInjection;
using Producer.Common;
using Producer.Common.Types;

namespace Producer.Infrastructure;

public static class VehicleContextDi
{
    public static IServiceCollection AddVehicleContext(this IServiceCollection services)
    {
        services.AddScoped<VehicleContextHolder>();
        services.AddScoped<IVehicleContext>(sp =>
        {
            var holder = sp.GetRequiredService<VehicleContextHolder>();
            return holder.Value ?? throw new InvalidOperationException(
                "VehicleContext was not initialized for this scope");
        });
        return services;
    }

    public static IServiceScope CreateVehicleScope(
        this IServiceProvider root, string vehicleId, DataSchemas version)
    {
        var scope = root.CreateScope();
        var holder = scope.ServiceProvider.GetRequiredService<VehicleContextHolder>();
        holder.Value = new VehicleContext { VehicleId = vehicleId, Version = version };
        return scope;
    }
}