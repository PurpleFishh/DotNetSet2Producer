using Generator.Generators;

namespace Producer.Business.Services.Interface.Generators;

public interface IVehicleIdGenerator
{
    public IValueGenerator<string> Get(string fixedId);
}