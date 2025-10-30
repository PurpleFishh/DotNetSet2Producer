using Generator.Random;

namespace Generator.Generators.Helper;

public class Dependent<T>(Func<IRandomSource, GenerationContext, T> fn) : IValueGenerator<T>
{
    public T Next(IRandomSource r, GenerationContext c) => fn(r, c);
}