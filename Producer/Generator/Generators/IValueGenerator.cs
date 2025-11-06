using Generator.Random;

namespace Generator.Generators;

public interface IValueGenerator<out T>
{
    T Next(IRandomSource rnd, IGenerationContext ctx);
}