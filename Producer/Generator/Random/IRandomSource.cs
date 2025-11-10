namespace Generator.Random;

public interface IRandomSource
{
    double NextDouble();             
    int NextInt(int minInclusive, int maxExclusive);
    long NextLong(long minInclusive, long maxExclusive);
}