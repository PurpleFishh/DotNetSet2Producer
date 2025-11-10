namespace Generator;

public interface IGenerationContext
{
    public void Set<T>(string key, T? value);

    public bool TryGet<T>(string key, out T? value);
}