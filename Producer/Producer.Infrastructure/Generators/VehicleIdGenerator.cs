using Generator.Generators;
using Generator.Generators.Helper;
using Producer.Business.Services.Interface.Generators;

namespace Producer.Infrastructure.Generators;

public class VehicleIdGenerator : IVehicleIdGenerator
{
    public IValueGenerator<string> Get(string fixedId)
        => new Dependent<string>((r, c) => fixedId);
}