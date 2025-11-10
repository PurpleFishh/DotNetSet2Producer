using Producer.Common.Config;
using Producer.Common.Types;

namespace Producer.Business.Services.Implementation.FileSystem;

public class FileSystemServiceOptions
{
    public string BaseFolder { get; private init; } = "inbox";
    public FileCompressionType FileCompression { get; private init; } = FileCompressionType.None;
    public int MaxRecords { get; private init; } = 5000;
    public long MaxBytes { get; private init; } = 10 * 1024 * 1024;
    public TimeSpan MaxAge { get; private init; } = TimeSpan.FromMinutes(1);
    public int BacklogThreshold { get; private init; } = 200;
    public TimeSpan BackoffBase { get; private init; } = TimeSpan.FromSeconds(5);
    public TimeSpan BackoffMax { get; private init; } = TimeSpan.FromSeconds(120);

    public static FileSystemServiceOptions From(ProducerOptions opt) => new()
    {
        BaseFolder = opt.Paths.BaseFolder,
        FileCompression = Enum.Parse<FileCompressionType>(opt.Io.Compression, true),
        MaxRecords = opt.RotationPolicy.MaxRecords,
        MaxBytes = opt.RotationPolicy.MaxBytes,
        MaxAge = TimeSpan.FromSeconds(opt.RotationPolicy.TimeWindowSeconds),
        BacklogThreshold = opt.WindowSize.BackpressureMaxUnprocessed,
        BackoffBase = TimeSpan.FromSeconds(opt.WindowSize.BackoffBaseSeconds),
        BackoffMax = TimeSpan.FromSeconds(opt.WindowSize.BackoffMaxSeconds)
    };
}