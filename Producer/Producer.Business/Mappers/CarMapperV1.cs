using Producer.Business.Entity;
using Producer.Common;

namespace Producer.Business.Mappers;

public class CarMapperV1 : ICarMapper
{
    public DataSchemas Version => DataSchemas.v1;
    public object Map(CarEntity result) => result.ToDtoV1();
}