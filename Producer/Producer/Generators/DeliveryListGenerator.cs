using Generator.Generators;
using Generator.Generators.Helper;

namespace Producer.Generators;

public class DeliveryListGenerator
{
    public static IValueGenerator<List<int>> Get()
    {
        return new Dependent<List<int>>((r, c) =>
        {
            if (!c.TryGet("DeliveryList", out List<int>? list))
            {
                list = [];
                c.Set("DeliveryList", list);
            }

            return [..list!];
        });
    }
}