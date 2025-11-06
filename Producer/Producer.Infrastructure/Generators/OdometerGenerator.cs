using Generator.Generators;
using Generator.Generators.Helper;
using Generator.Generators.ValueGenerator;
using Generator.Random;
using Producer.Business.Entity;
using Producer.Business.Services.Interface.Generators;

namespace Producer.Infrastructure.Generators;

public class OdometerGenerator : IOdometerGenerator
{
    public IValueGenerator<double> Get()
    {
        var odo = new JitterAroundPrev(
            key: nameof(CarEntity.Odometer),
            min: 0,
            max: double.MaxValue,
            maxDelta: 0.2,
            mode: JitterAroundPrevMode.AbovePrev,
            fallback: new DoubleGenerator(15432.6, 15433.0, Shapes.Tri(0.1))
        );
        return odo;
    }
}