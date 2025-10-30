namespace Producer.CarDataGenerator;

public static class AutoMapper
{
    public static T MapTo<T>(IReadOnlyDictionary<string, object> values,
        Dictionary<string, Func<object>> predefinedValues)
    {
        var constructorInfo = typeof(T).GetConstructors().First();
        var args = constructorInfo.GetParameters().Select(p =>
        {
            if (predefinedValues.TryGetValue(p.Name!, out var getter))
                return getter();
            return values.TryGetValue(p.Name!, out var v) ? v : GetDefault(p.ParameterType);
        }).ToArray();

        return (T)constructorInfo.Invoke(args);
    }

    private static object? GetDefault(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;
}