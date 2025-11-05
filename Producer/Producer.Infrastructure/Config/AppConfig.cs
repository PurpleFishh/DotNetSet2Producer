using Microsoft.Extensions.Configuration;

namespace Producer.Infrastructure.Config;

public static class AppConfig
{
    public static ProducerOptions? Current { get; private set; }

    public static void Initialize(IConfiguration configuration)
    {
        Current = configuration.GetSection("Producer").Get<ProducerOptions>()
                  ?? throw new InvalidOperationException("Missing 'Producer' config section");
    }
}