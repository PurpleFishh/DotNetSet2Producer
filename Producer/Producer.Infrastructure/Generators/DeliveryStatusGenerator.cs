using Generator.Generators;
using Generator.Generators.Helper;

namespace Producer.Infrastructure.Generators;

public static class DeliveryStatusGenerator
{
    /// <summary>
    /// Orchestrates daily pickups & deliveries. Side-effects:
    /// - Updates ctx["DeliveryList"] : List<int>
    /// </summary>
    public static IValueGenerator<DeliveryStatus> Get(double deliverProb = 0.1)
    {
        return new Dependent<DeliveryStatus>((r, c) =>
        {
            var middayTime = c.TryGet("MiddayTime", out TimeOnly midTime) ? midTime : GeneratorConstants.MiddayTime;
            var dayStartTime = c.TryGet("DayStartTime", out TimeOnly startTIme)
                ? startTIme
                : GeneratorConstants.DayStartTime;
            var time = c.TryGet("TsUtc", out DateTime timeNow) ? timeNow : DateTime.UtcNow;

            var deliveryList = c.TryGet("DeliveryList", out List<int>? l) ? l! : [];

            if (TimeOnly.FromDateTime(time) == dayStartTime || TimeOnly.FromDateTime(time) == middayTime)
                return DeliveryStatus.PickUp;

            // deliveries
            if (deliveryList.Count > 0 && r.NextDouble() < deliverProb &&
                (time.Hour < middayTime.Hour && time.Hour > 9 || time.Hour > middayTime.Hour + 1))
            {
                var idx = r.NextInt(0, deliveryList.Count);
                deliveryList.RemoveAt(idx);
                c.Set("DeliveryList", deliveryList);
                return DeliveryStatus.Completed;
            }

            return DeliveryStatus.InProgress;
        });
    }
}