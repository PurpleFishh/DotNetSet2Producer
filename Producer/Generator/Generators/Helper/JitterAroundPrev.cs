using Generator.Random;

namespace Generator.Generators.Helper;

public enum JitterAroundPrevMode
{
    AroundPrev,
    AbovePrev,
    BelowPrev
}

public class JitterAroundPrev(
    string key,
    double min,
    double max,
    double maxDelta,
    JitterAroundPrevMode mode,
    IValueGenerator<double> fallback)
    : IValueGenerator<double>
{
    public JitterAroundPrev(string key, double min, double max, double maxDelta, IValueGenerator<double> fallback)
        : this(key, min, max, maxDelta, JitterAroundPrevMode.AroundPrev, fallback)
    {
    }

    public double Next(IRandomSource rnd, GenerationContext ctx)
    {
        if (!ctx.TryGet<double>(key, out var prev))
            return fallback.Next(rnd, ctx);

        var delta = mode switch
        {
            JitterAroundPrevMode.AroundPrev => (rnd.NextDouble() * 2 - 1) * maxDelta,
            JitterAroundPrevMode.AbovePrev => rnd.NextDouble() * maxDelta,
            JitterAroundPrevMode.BelowPrev => -rnd.NextDouble() * maxDelta,
            _ => 0
        };

        var v = Math.Clamp(prev + delta, min, max);
        ctx.Set(key, v);
        return v;
    }
}