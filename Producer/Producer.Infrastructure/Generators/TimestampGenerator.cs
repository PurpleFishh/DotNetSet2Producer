using Generator.Generators;
using Generator.Generators.Helper;
using Producer.Business.Entity;
using Producer.Business.Services.Interface.Generators;

namespace Producer.Infrastructure.Generators;

public class TimestampGenerator : ITimestampGenerator
{
    public IValueGenerator<DateTime> Get(int minutesBetweenEvents = 1, int secondsBetweenEvents = 0)
        => new Dependent<DateTime>((r, c) =>
        {
            if (!c.TryGet(nameof(CarEntity.TsUtc), out DateTime tsUtc))
                return DateTime.Today.AddHours(8); // start at 8am today
            tsUtc = tsUtc.AddMinutes(minutesBetweenEvents).AddSeconds(secondsBetweenEvents);
            if (tsUtc.Hour >= 16)
                tsUtc = DateTime.Today.AddDays((tsUtc - DateTime.UtcNow).Days + 1).AddHours(8); // next day at 8am
            return tsUtc;
        });
}