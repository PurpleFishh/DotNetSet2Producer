using Generator.Generators;
using Generator.Generators.Helper;

namespace Producer.Generators;

public static class TimestampGenerator
{
    public static IValueGenerator<DateTime> Get(int minutesBetweenEvents = 1, int secondsBetweenEvents = 0)
        => new Dependent<DateTime>((r, c) =>
        {
            if (!c.TryGet("TsUtc", out DateTime tsUtc))
                return DateTime.Today.AddHours(8); // start at 8am today
            tsUtc = tsUtc.AddMinutes(minutesBetweenEvents).AddSeconds(secondsBetweenEvents);
            if (tsUtc.Hour >= 16)
                tsUtc = DateTime.Today.AddDays((tsUtc - DateTime.UtcNow).Days + 1).AddHours(8); // next day at 8am
            return tsUtc;
        });
}