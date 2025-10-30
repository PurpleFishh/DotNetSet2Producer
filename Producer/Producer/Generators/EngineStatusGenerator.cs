using Generator.Generators;
using Generator.Generators.Helper;
using Producer.Generator;

namespace Producer.CarDataGenerator.Generators;

public static class EngineStatusGenerator
{
    public static IValueGenerator<bool> Get()
    {
        var engineOn = new Dependent<bool>((r, c) => r.NextDouble() < 0.92);
        return engineOn;
    }
}