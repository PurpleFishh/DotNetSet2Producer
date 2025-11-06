using Generator.Random;

namespace Generator.Generators.Helper;

public class Dependent<T>(Func<IRandomSource, IGenerationContext, T> fn) : IValueGenerator<T>
{
    public T Next(IRandomSource r, IGenerationContext c) => fn(r, c);
}