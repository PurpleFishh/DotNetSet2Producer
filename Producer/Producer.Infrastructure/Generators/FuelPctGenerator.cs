using Generator.Generators;
using Generator.Generators.Helper;
using Generator.Generators.ValueGenerator;
using Producer.Business.Entity;
using Producer.Business.Services.Interface.Generators;

namespace Producer.Infrastructure.Generators;

public class FuelPctGenerator : IFuelPctGenerator
{
    private static DoubleGenerator _doubleGeneretor;

    public IValueGenerator<double> Get(double minDrop = 0.05, double maxDrop = 0.1)
    {
        _doubleGeneretor = new DoubleGenerator(minDrop, maxDrop);
        return new Dependent<double>((r, c) =>
        {
            // refill on pickup
            if (c.TryGet(nameof(CarEntity.DeliveryStatus), out DeliveryStatus st) && st == DeliveryStatus.PickUp)
            {
                return 100.0;
            }

            var prev = c.TryGet(nameof(CarEntity.FuelPct), out double fp) ? fp : 100.0;
            var next = Math.Max(0.0, prev - _doubleGeneretor.Next(r, c));

            // next = Math.Round(next, 1);
            return next;
        });
    }
}