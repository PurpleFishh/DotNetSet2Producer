using System.Security.Cryptography;
using Generator.Random;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Producer.Business.Services.Interface;
using Producer.Common.Config;
using Producer.Common.Types;

namespace Producer.Business.Services.Implementation;

public class FaultInjectionService(IOptions<FaultInjectionOptions> options, ILogger<FaultInjectionService> logger)
    : IFaultInjectionService
{
    private readonly IRandomSource _rnd = new DefaultRandomSource();
    public readonly FaultInjectionOptions Options = options.Value;

    public FaultPhaseType GetRandomPhase()
        => _rnd.NextInt(0, 1) == 0 ? FaultPhaseType.BeforeHash : FaultPhaseType.AfterHash;

    public bool ShouldDropRecord() =>
        Options.Enabled && Options.DropRecordProb > 0 && _rnd.NextDouble() < Options.DropRecordProb;

    public void MaybeCorruptTail(string path)
    {
        if (!Options.Enabled) return;

        if (Options.TruncateProb > 0 && _rnd.NextDouble() < Options.TruncateProb)
        {
            TruncateTail(path, Options.TruncateTailBytes);
            logger.LogInformation($"Truncated last {Options.TruncateTailBytes} bytes");
        }

        if (Options.CorruptTailProb > 0 && _rnd.NextDouble() < Options.CorruptTailProb)
        {
            CorruptTail(path, Options.CorruptTailBytes);
            logger.LogInformation($"Truncated last {Options.TruncateTailBytes} bytes");
        }
    }

    private static void TruncateTail(string path, int bytes)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        var newLen = Math.Max(0, fs.Length - bytes);
        fs.SetLength(newLen);
        fs.Flush();
    }

    private static void CorruptTail(string path, int bytes)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        var n = Math.Min(bytes, (int)fs.Length);
        if (n <= 0) return;

        var buf = new byte[n];
        RandomNumberGenerator.Fill(buf);
        fs.Seek(-n, SeekOrigin.End);
        fs.Write(buf, 0, n);
        fs.Flush();
    }
}