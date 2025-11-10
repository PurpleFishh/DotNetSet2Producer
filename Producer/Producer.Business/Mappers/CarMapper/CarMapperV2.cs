using Producer.Business.Entity;
using Producer.Common.Types;

namespace Producer.Business.Mappers.CarMapper;

public class CarMapperV2 : ICarMapper
{
    public DataSchemas Version => DataSchemas.v2;
    public object Map(CarEntity result) => result.ToDtoV2();
}