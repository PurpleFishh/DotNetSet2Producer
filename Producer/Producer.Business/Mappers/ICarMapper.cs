using Producer.Business.Entity;
using Producer.Common;

namespace Producer.Business.Mappers;

public interface ICarMapper
{
    DataSchemas Version { get; }
    object Map(CarEntity result);
}