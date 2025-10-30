using Generator.Random;

namespace Generator.Generators.ValueGenerator;

public class CarIdGenerator(string prefix = "V-", int digits = 3, int min = 0, int max = 999)
    : IValueGenerator<string>
{
    public string Prefix { get; } = prefix;
    public int Digits { get; } = digits;
    public int Min { get; } = min;
    public int Max { get; } = max;

    public string Next(IRandomSource rnd, GenerationContext ctx)
    {
        return $"{Prefix}{rnd.NextInt(Min, Max + 1).ToString().PadLeft(Digits, '0')}";
    }
}