using Generator.Generators;
using Generator.Generators.ValueGenerator;
using Generator.Random;

namespace Producer.Generators;

public static class FuelPctGenerator
{
    public static IValueGenerator<double> Get()
    {
        var fuelPct = new DoubleGenerator(0, 100, Shapes.Mid());
        return fuelPct;
    }
}