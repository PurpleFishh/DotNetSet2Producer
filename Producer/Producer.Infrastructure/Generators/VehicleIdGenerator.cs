using Generator.Generators;
using Generator.Generators.Helper;

namespace Producer.Infrastructure.Generators;

public class VehicleIdGenerator
{
    public static IValueGenerator<string> Get(string fixedId)
        => new Dependent<string>((r, c) => fixedId);
}