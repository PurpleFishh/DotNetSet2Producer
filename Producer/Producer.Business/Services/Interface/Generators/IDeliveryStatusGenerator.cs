using Generator.Generators;
using Producer.Common.Types;

namespace Producer.Business.Services.Interface.Generators;

public interface IDeliveryStatusGenerator
{
    public IValueGenerator<DeliveryStatus> Get(double deliverProb = 0.1);
}