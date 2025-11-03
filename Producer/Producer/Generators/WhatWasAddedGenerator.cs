using Generator.Generators;
using Generator.Generators.Helper;

namespace Producer.Generators;

public class WhatWasAddedGenerator
{
    public static IValueGenerator<List<int>?> Get()
    {
        return new Dependent<List<int>?>((r, c) =>
        {
            // set by DeliveryStatusGenerator on PickUp ticks
            return c.TryGet("WhatWasAdded", out List<int>? added) ? [..added!] : null;
        });
    }
}