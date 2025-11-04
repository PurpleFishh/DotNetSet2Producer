using System.Security.Cryptography;
using Generator.Random;
using Producer.Config;

namespace Producer.Services;

public sealed class FaultInjectionService
{
    private readonly IRandomSource _rnd = new DefaultRandomSource();
    public readonly FaultInjectionOptions Options = AppConfig.Current!.FaultInjection;

    public FaultPhase GetRandomPhase()
        => _rnd.NextInt(0, 1) == 0 ? FaultPhase.BeforeHash : FaultPhase.AfterHash;
    
    public bool ShouldDropRecord() =>
        Options.Enabled && Options.DropRecordProb > 0 && _rnd.NextDouble() < Options.DropRecordProb;

    public string? MaybeCorruptTail(string path)
    {
        if (!Options.Enabled) return null;

        if (Options.TruncateProb > 0 && _rnd.NextDouble() < Options.TruncateProb)
        {
            TruncateTail(path, Options.TruncateTailBytes);
            return $"Truncated last {Options.TruncateTailBytes} bytes";
        }

        if (Options.CorruptTailProb > 0 && _rnd.NextDouble() < Options.CorruptTailProb)
        {
            CorruptTail(path, Options.CorruptTailBytes);
            return $"Corrupted last {Options.CorruptTailBytes} bytes";
        }

        return null;
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