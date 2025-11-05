using Generator.Generators;
using Generator.Generators.Helper;

namespace Producer.Infrastructure.Generators.Deprecated;

public static class EngineStatusGenerator
{
    public static IValueGenerator<bool> Get()
    {
        var engineOn = new Dependent<bool>((r, c) => r.NextDouble() < 0.92);
        return engineOn;
    }
}