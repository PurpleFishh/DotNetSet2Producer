using Generator.Generators;
using Generator.Generators.Helper;
using Producer.Entity;

namespace Producer.Generators;

public static class GpsGenerator
{
    // public static IValueGenerator<GpsInfo> Get()
    // {
    //     var gps = new Dependent<GpsInfo>((r, c) =>
    //     {
    //         var (clat, clon) = (52.5201, 13.4049);
    //         var dLat = (r.NextDouble() - 0.5) * 0.003;
    //         var dLon = (r.NextDouble() - 0.5) * 0.006;
    //         return new GpsInfo(clat + dLat, clon + dLon);
    //     });
    //     return gps;
    // }
}