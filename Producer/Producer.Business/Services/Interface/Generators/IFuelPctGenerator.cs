using Generator.Generators;

namespace Producer.Business.Services.Interface.Generators;

public interface IFuelPctGenerator
{
    public IValueGenerator<double> Get(double minDrop = 0.05, double maxDrop = 0.1);
}