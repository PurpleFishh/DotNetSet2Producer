using Generator.Generators;
using Generator.Generators.Helper;
using Generator.Generators.ValueGenerator;
using Generator.Random;
using Producer.Generator;

namespace Producer.CarDataGenerator.Generators;

public static class CoolantInfoGenerator
{
    public static IValueGenerator<double> Get()
    {
        var coolantOn = new DoubleGenerator(85, 110, Shapes.Mid());
        var coolantOff = new DoubleGenerator(60, 90, Shapes.Mid());
        var coolant = new Dependent<double>((r, c) =>
        {
            c.TryGet("engineOn", out bool on);
            return (on ? coolantOn : coolantOff).Next(r, c);
        });
        return coolant;
    }
}