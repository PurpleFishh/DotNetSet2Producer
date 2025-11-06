using Generator.Generators;
using Generator.Generators.Helper;
using Microsoft.Extensions.Options;
using Producer.Business.Entity;
using Producer.Business.Services.Interface.Generators;

namespace Producer.Infrastructure.Generators;

public class WhatWasAddedGenerator(IOptions<GeneratorTimeOptions> times) : IWhatWasAddedGenerator
{
    private readonly GeneratorTimeOptions _times = times.Value;
    private const string NextPackageIdKey = "NextPackageId";

    public IValueGenerator<List<int>?> Get(int morningLoad = 30, int middayLoad = 15)
    {
        return new Dependent<List<int>?>((r, c) =>
        {
            var dateNow = c.TryGet(nameof(CarEntity.TsUtc), out DateTime tsUtc) ? tsUtc : DateTime.UtcNow;
            var nextId = c.TryGet(NextPackageIdKey, out int nid) ? nid : 1;

            var timeNow = TimeOnly.FromDateTime(dateNow);

            if (timeNow == _times.DayStartTime)
            {
                var addedPackages = Enumerable.Range(nextId, morningLoad).ToList();
                nextId += morningLoad;
                c.Set(NextPackageIdKey, nextId);
                return addedPackages;
            }

            if (timeNow == _times.MiddayTime)
            {
                var addedPackages = Enumerable.Range(nextId, middayLoad).ToList();
                nextId += middayLoad;
                c.Set(NextPackageIdKey, nextId);
                return addedPackages;
            }

            return [];
        });
    }
}