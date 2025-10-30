using Generator.Generators;
using Generator.Generators.Helper;
using Producer.Generator;

namespace Producer.CarDataGenerator.Generators;

public static class GpsGenerator
{
    public static IValueGenerator<(double, double)> Get()
    {
        var gps = new Dependent<(double, double)>((r, c) =>
        {
            var (clat, clon) = (52.5201, 13.4049);
            var dLat = (r.NextDouble() - 0.5) * 0.003;
            var dLon = (r.NextDouble() - 0.5) * 0.006;
            return (clat + dLat, clon + dLon);
        });
        return gps;
    }
}