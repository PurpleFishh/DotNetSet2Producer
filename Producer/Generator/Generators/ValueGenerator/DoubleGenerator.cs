using Generator.Random;

namespace Generator.Generators.ValueGenerator;

public class DoubleGenerator(double min, double max, Func<double, double>? shape01 = null)
    : IValueGenerator<double>
{
    public double Min { get; } = min;
    public double Max { get; } = max;
    public Func<double, double>? Shape01 { get; } = shape01; // maps U[0,1)->[0,1]

    public double Next(IRandomSource rnd, IGenerationContext ctx)
    {
        var u = Shape01?.Invoke(rnd.NextDouble()) ?? rnd.NextDouble();
        return Min + (Max - Min) * Math.Clamp(u, 0, 1);
    }
}