namespace Producer.Common;

public class VehicleContext: IVehicleContext
{
    public string VehicleId { get; init; } = default!;
    public DataSchemas Version   { get; init; } = default!;
}