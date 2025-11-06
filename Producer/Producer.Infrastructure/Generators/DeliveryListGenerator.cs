using Generator.Generators;
using Generator.Generators.Helper;
using Producer.Business.Entity;
using Producer.Business.Services.Interface.Generators;

namespace Producer.Infrastructure.Generators;

public class DeliveryListGenerator : IDeliveryListGenerator
{
    public IValueGenerator<List<int>> Get()
    {
        return new Dependent<List<int>>((r, c) =>
        {
            var packages = c.TryGet(nameof(CarEntity.DeliveryList), out List<int>? list) ? list! : [];

            var addedPackages = c.TryGet(nameof(CarEntity.WhatWasAdded), out List<int>? added) ? added! : [];
            packages.AddRange(addedPackages);
            return packages;
        });
    }
}