using Generator.Generators;

namespace Producer.Business.Services.Interface.Generators;

public interface IWhatWasAddedGenerator
{
    public IValueGenerator<List<int>?> Get(int morningLoad = 30, int middayLoad = 15);
}