using Producer.Business.Entity;
using Producer.Business.Mappers.CarMapper;
using Producer.Common.Types;

namespace Producer.Business.Mappers;

public class CarMapperV1 : ICarMapper
{
    public DataSchemas Version => DataSchemas.v1;
    public object Map(CarEntity result) => result.ToDtoV1();
}