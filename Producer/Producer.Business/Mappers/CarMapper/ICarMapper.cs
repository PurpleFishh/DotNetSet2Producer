using Producer.Business.Entity;
using Producer.Common.Types;

namespace Producer.Business.Mappers.CarMapper;

public interface ICarMapper
{
    DataSchemas Version { get; }
    object Map(CarEntity result);
}