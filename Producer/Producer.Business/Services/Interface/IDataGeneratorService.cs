using Generator;
using Producer.Business.Entity;

namespace Producer.Business.Services.Interface;

public interface IDataGeneratorService<T>
{
    public RecordBuilder<T> GetGenerator();
}