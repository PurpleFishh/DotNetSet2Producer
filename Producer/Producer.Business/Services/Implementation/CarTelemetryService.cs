using Generator;
using Generator.Random;
using Producer.Business.Entity;
using Producer.Business.Mappers;
using Producer.Business.Services.Interface;
using Producer.Business.Services.Interface.Generators;
using Producer.Common;

namespace Producer.Business.Services.Implementation;

public class CarTelemetryService(
    IRandomSource random,
    IGenerationContext context,
    IDataGeneratorService<CarEntity> builder)
    : ICarTelemetryService
{
    private readonly RecordBuilder<CarEntity> _builder = builder.GetGenerator();

    public CarEntity GenerateValue() => _builder.Build(random, context);
}