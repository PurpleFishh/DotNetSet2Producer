using Generator.Generators;
using Producer.Business.Entity;

namespace Producer.Business.Services.Interface.Generators;

public interface IDeliveryStatusGenerator
{
    public IValueGenerator<DeliveryStatus> Get(double deliverProb = 0.1);
}