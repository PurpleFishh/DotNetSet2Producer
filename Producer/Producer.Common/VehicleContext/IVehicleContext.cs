using Producer.Common.Types;

namespace Producer.Common;

public interface IVehicleContext
{
    string VehicleId { get; }
    DataSchemas Version { get; }
}