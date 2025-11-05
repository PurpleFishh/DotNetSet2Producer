using Microsoft.Extensions.Logging;

namespace Producer.Infrastructure.Utils;

public static class AppLogger
{
    private static ILoggerFactory? _factory;

    public static void Init(LogLevel level = LogLevel.Information)
    {
        if (_factory != null) return;

        _factory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole(options =>
                {
                    options.TimestampFormat = "[HH:mm:ss] ";
                    options.IncludeScopes = false;
                })
                .SetMinimumLevel(level);
        });
    }

    public static ILogger<T> Get<T>()
    {
        if (_factory == null)
            Init();
        return _factory!.CreateLogger<T>();
    }

    public static ILogger Get(string category)
    {
        if (_factory == null)
            Init();
        return _factory!.CreateLogger(category);
    }

    public static void Shutdown()
    {
        if (_factory is IDisposable disp)
            disp.Dispose();
        _factory = null;
    }
}