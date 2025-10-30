using Generator.Random;

namespace Generator.Generators.Helper;

public class Round<T>(IValueGenerator<double> inner, int decimals) : IValueGenerator<T>
    where T : struct, IConvertible
{
    public T Next(IRandomSource rnd, GenerationContext ctx)
        => (T)Convert.ChangeType(Math.Round(inner.Next(rnd, ctx), decimals), typeof(T));
}