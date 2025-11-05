using Generator.Generators;
using Generator.Generators.Helper;

namespace Producer.Infrastructure.Generators;

public static class WhatWasAddedGenerator
{
    public static IValueGenerator<List<int>?> Get(int morningLoad = 30, int middayLoad = 15)
    {
        return new Dependent<List<int>?>((r, c) =>
        {
            var middayTime = c.TryGet("MiddayTime", out TimeOnly midTime) ? midTime : GeneratorConstants.MiddayTime;
            var dayStartTime = c.TryGet("DayStartTime", out TimeOnly start) ? start : GeneratorConstants.DayStartTime;
            var dateNow = c.TryGet("TsUtc", out DateTime tsUtc) ? tsUtc : DateTime.UtcNow;
            var nextId = c.TryGet("NextPackageId", out int nid) ? nid : 1;
            
            var timeNow = TimeOnly.FromDateTime(dateNow);
            
            if (timeNow == dayStartTime)
            {
                var addedPackages = Enumerable.Range(nextId, morningLoad).ToList();
                nextId += morningLoad;
                c.Set("NextPackageId", nextId);
                return addedPackages;
            }

            if (timeNow == middayTime)
            {
                var addedPackages = Enumerable.Range(nextId, middayLoad).ToList();
                nextId += middayLoad;
                c.Set("NextPackageId", nextId);
                return addedPackages;
            }

            return [];
        });
    }
}