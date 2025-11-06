using Producer.Business.Entity;
using Producer.Common;

namespace Producer.Business.Mappers;

public class CarMapperV2 : ICarMapper
{
    public DataSchemas Version => DataSchemas.v2;
    public object Map(CarEntity result) => result.ToDtoV2();
}