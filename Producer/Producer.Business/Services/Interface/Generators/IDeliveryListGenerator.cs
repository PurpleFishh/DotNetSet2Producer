using Generator.Generators;

namespace Producer.Business.Services.Interface.Generators;

public interface IDeliveryListGenerator
{
    public IValueGenerator<List<int>> Get();
}