namespace Generator.Random;

public class DefaultRandomSource(int? seed = null) : IRandomSource
{
    private readonly System.Random _r = seed is null ? new System.Random() : new System.Random(seed.Value);
    public double NextDouble() => _r.NextDouble();
    public int NextInt(int min, int max) => _r.Next(min, max);

    public long NextLong(long min, long max)
    {
        var span = max - min;
        return min + (long)Math.Floor(_r.NextDouble() * span);
    }
}