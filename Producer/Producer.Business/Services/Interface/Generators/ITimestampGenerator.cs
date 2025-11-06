using Generator.Generators;

namespace Producer.Business.Services.Interface.Generators;

public interface ITimestampGenerator
{
    public IValueGenerator<DateTime> Get(int minutesBetweenEvents, int secondsBetweenEvents );
}