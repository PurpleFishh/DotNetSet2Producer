using Generator;
using Generator.Random;
using Microsoft.Extensions.DependencyInjection;
using Producer.Business.Entity;
using Producer.Business.Mappers;
using Producer.Business.Services.Implementation;
using Producer.Business.Services.Implementation.Backpressure;
using Producer.Business.Services.Implementation.FileSystem;
using Producer.Business.Services.Interface;
using Producer.Common;

namespace Producer.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IFileSystemService, FileSystemService>();
        services.AddTransient<IBackpressureService, BackpressureService>();
        services.AddTransient<IFaultInjectionService, FaultInjectionService>();
        services.AddTransient<IFileMetadataService, FileMetadataService>();
        services.AddTransient<ICarTelemetryService, CarTelemetryService>();
        services.AddKeyedSingleton<ICarMapper, CarMapperV1>(DataSchemas.v1);
        services.AddKeyedSingleton<ICarMapper, CarMapperV2>(DataSchemas.v2);
        services.AddTransient<IDataGeneratorService<CarEntity>, CarDataGeneratorService>();

        services.AddSingleton<IGenerationContext, GenerationContext>();
        services.AddTransient<IRandomSource, DefaultRandomSource>();

        return services;
    }
}