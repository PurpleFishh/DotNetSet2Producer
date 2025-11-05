using Generator.Generators;
using Generator.Generators.Helper;
using Generator.Generators.ValueGenerator;
using Generator.Random;

namespace Producer.Infrastructure.Generators.Deprecated;

public static class SpeedGenerator
{
    public static IValueGenerator<double> Get()
    {
        var speedDriving = new DoubleGenerator(5, 130, Shapes.Tri(0.4));
        var speedIdle = new DoubleGenerator(0, 2, Shapes.Tri(0.1));
        var speed = new Dependent<double>((r, c) =>
        {
            var on = c.TryGet("engineOn", out bool v) ? v : true;
            return (on ? speedDriving : speedIdle).Next(r, c);
        });
        return speed;
    }
}