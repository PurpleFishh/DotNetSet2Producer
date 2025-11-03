namespace Generator;

public class GenerationContext
{
    private readonly Dictionary<string, object?> _bag = new();
    public void Set<T>(string key, T? value) => _bag[key] = value;

    public bool TryGet<T>(string key, out T? value)
    {
        if (_bag.TryGetValue(key, out var v) && v is T t)
        {
            value = t;
            return true;
        }

        value = default;
        return false;
    }
}