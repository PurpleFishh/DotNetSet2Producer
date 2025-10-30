using Generator.Generators;
using Generator.Random;

namespace Generator;

public class RecordBuilder<T>(Func<IReadOnlyDictionary<string, object>, T> objectMapper)
{
    private readonly List<(string key, Func<IRandomSource, GenerationContext, object>)> _buildSteps = [];

    public RecordBuilder<T> AddStep<TProp>(string key, IValueGenerator<TProp> gen)
    {
        _buildSteps.Add((key, (r, ctx) =>
            {
                var value = gen.Next(r, ctx)!;
                ctx.Set(key, value);
                return value;
            })
        );
        return this;
    }

    public T Build(IRandomSource rnd, GenerationContext? ctx = null)
    {
        ctx ??= new GenerationContext();
        var map = new Dictionary<string, object>();
        foreach (var (key, generator) in _buildSteps)
            map[key] = generator(rnd, ctx);
        return objectMapper(map);
    }
}