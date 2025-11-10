using Generator.Generators;
using Generator.Generators.Helper;
using Microsoft.Extensions.Options;
using Producer.Business.Entity;
using Producer.Business.Services.Interface.Generators;
using Producer.Common.Types;

namespace Producer.Infrastructure.Generators;

public class DeliveryStatusGenerator(IOptions<GeneratorTimeOptions> times) : IDeliveryStatusGenerator
{
    private readonly GeneratorTimeOptions _times = times.Value;

    /// <summary>
    /// Orchestrates daily pickups & deliveries. Side-effects:
    /// - Updates ctx["DeliveryList"] : List<int>
    /// </summary>
    public IValueGenerator<DeliveryStatus> Get(double deliverProb = 0.1)
    {
        return new Dependent<DeliveryStatus>((r, c) =>
        {
            var time = c.TryGet(nameof(CarEntity.TsUtc), out DateTime timeNow) ? timeNow : DateTime.UtcNow;

            var deliveryList = c.TryGet(nameof(CarEntity.DeliveryList), out List<int>? l) ? l! : [];

            if (TimeOnly.FromDateTime(time) == _times.DayStartTime || TimeOnly.FromDateTime(time) == _times.MiddayTime)
                return DeliveryStatus.PickUp;

            // deliveries
            if (deliveryList.Count > 0 && r.NextDouble() < deliverProb &&
                (time.Hour < _times.MiddayTime.Hour && time.Hour > 9 || time.Hour > _times.MiddayTime.Hour + 1))
            {
                var idx = r.NextInt(0, deliveryList.Count);
                deliveryList.RemoveAt(idx);
                c.Set(nameof(CarEntity.DeliveryList), deliveryList);
                return DeliveryStatus.Completed;
            }

            return DeliveryStatus.InProgress;
        });
    }
}