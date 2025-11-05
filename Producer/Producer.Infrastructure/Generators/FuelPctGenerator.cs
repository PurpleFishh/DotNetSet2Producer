using Generator.Generators;
using Generator.Generators.Helper;
using Generator.Generators.ValueGenerator;

namespace Producer.Infrastructure.Generators;

public static class FuelPctGenerator
{
    private static DoubleGenerator doubleGeneretor;

    public static IValueGenerator<double> Get(double minDrop = 0.05, double maxDrop = 0.1)
    {
        doubleGeneretor = new DoubleGenerator(minDrop, maxDrop);
        return new Dependent<double>((r, c) =>
        {
            // refill on pickup
            if (c.TryGet("DeliveryStatus", out DeliveryStatus st) && st == DeliveryStatus.PickUp)
            {
                return 100.0;
            }

            var prev = c.TryGet("FuelPct", out double fp) ? fp : 100.0;
            var next = Math.Max(0.0, prev - doubleGeneretor.Next(r, c));

            // next = Math.Round(next, 1);
            return next;
        });
    }
}