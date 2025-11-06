using Generator.Generators;

namespace Producer.Business.Services.Interface.Generators;

public interface IOdometerGenerator
{
    public IValueGenerator<double> Get();
}