using Generator.Generators;
using Generator.Generators.Helper;
using Generator.Generators.ValueGenerator;
using Generator.Random;

namespace Producer.Generators;

public static class OdoInfoGenerator
{
    public static IValueGenerator<double> Get()
    {
        var odo = new JitterAroundPrev("odoKm", 0, double.MaxValue, maxDelta: 0.2, JitterAroundPrevMode.AbovePrev,
            fallback: new DoubleGenerator(15432.6, 15433.0, Shapes.Tri(0.1)));

        return odo;
    }
}