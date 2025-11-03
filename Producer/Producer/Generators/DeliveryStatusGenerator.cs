using Generator.Generators;
using Generator.Generators.Helper;
using Generator.Generators.ValueGenerator;
using Generator.Random;
using Producer.Entity;

namespace Producer.Generators;

public static class DeliveryStatusGenerator
{
    /// <summary>
    /// Orchestrates daily pickups & deliveries. Side-effects:
    /// - Updates ctx["DeliveryList"] : List<int>
    /// - Updates ctx["WhatWasAdded"] : List<int> or null
    /// Also keeps ctx["NextPackageId"], ctx["TickIndex"] counters
    /// </summary>
    public static IValueGenerator<DeliveryStatus> Get(
        int morningLoad = 30,
        int middayLoad = 15,
        double deliverProb = 0.1)
    {
        return new Dependent<DeliveryStatus>((r, c) =>
        {
            var middayTime = c.TryGet("MiddayTime", out TimeOnly midTime) ? midTime : new TimeOnly(12, 00, 00);
            var time = c.TryGet("TsUtc", out DateTime timeNow) ? timeNow : DateTime.UtcNow;

            var deliveryList = c.TryGet("DeliveryList", out List<int>? l) ? l! : [];
            var nextId = c.TryGet("NextPackageId", out int nid) ? nid : 1;

            if (time is { Hour: 8, Minute: 0, Second: 0, Millisecond: 0 })
            {
                var added = Enumerable.Range(nextId, morningLoad).ToList();
                nextId += morningLoad;
                deliveryList.AddRange(added);
                c.Set("DeliveryList", deliveryList);
                c.Set("WhatWasAdded", added);
                c.Set("NextPackageId", nextId);
                // c.Set("TickIndex", time + 1);
                return DeliveryStatus.PickUp;
            }

            // midday pickup
            if (TimeOnly.FromDateTime(time) == middayTime)
            {
                var added = Enumerable.Range(nextId, middayLoad).ToList();
                nextId += middayLoad;
                deliveryList.AddRange(added);
                c.Set("DeliveryList", deliveryList);
                c.Set("WhatWasAdded", added);
                c.Set("NextPackageId", nextId);
                // c.Set("TickIndex", time + 1);
                return DeliveryStatus.PickUp;
            }

            // deliveries
            if (deliveryList.Count > 0 && r.NextDouble() < deliverProb &&
                (time.Hour < middayTime.Hour && time.Hour > 9 || time.Hour > middayTime.Hour + 1))
            {
                var idx = r.NextInt(0, deliveryList.Count);
                deliveryList.RemoveAt(idx);
                c.Set("DeliveryList", deliveryList);
                c.Set<object>("WhatWasAdded", null);
                // c.Set("TickIndex", time + 1);
                return DeliveryStatus.Completed;
            }

            // heartbeat / driving
            c.Set("DeliveryList", deliveryList);
            c.Set<object>("WhatWasAdded", null);
            // c.Set("TickIndex", time + 1);
            return DeliveryStatus.InProgress;
        });
    }
}