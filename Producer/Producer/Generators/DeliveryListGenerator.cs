using Generator.Generators;
using Generator.Generators.Helper;

namespace Producer.Generators;

public class DeliveryListGenerator
{
    public static IValueGenerator<List<int>> Get()
    {
        return new Dependent<List<int>>((r, c) =>
        {
            var packages = c.TryGet("DeliveryList", out List<int>? list) ? list! : [];

            var addedPackages = c.TryGet("WhatWasAdded", out List<int>? added) ? added! : [];
            packages.AddRange(addedPackages);
            return packages;
        });
    }
}