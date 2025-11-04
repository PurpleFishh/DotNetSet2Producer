using Microsoft.Extensions.Logging;
using Producer.Utils;

namespace Producer.Services;

public class BackpressureService(
    string baseFolder,
    string vehicleId,
    int backlogThreshold,
    TimeSpan backoffBase,
    TimeSpan backoffMax)
{
    
    private readonly ILogger<BackpressureService> _logger = AppLogger.Get<BackpressureService>();
    
    public async Task ApplyBackpressureAsync()
    {
        // var folder = Path.Combine(baseFolder, vehicleId);
        var folder = baseFolder;
        var unprocessed = CountUnprocessedFiles(folder);

        if (unprocessed <= backlogThreshold)
            return;

        var over = Math.Clamp(unprocessed - backlogThreshold, 1, 8);
        var baseMs = backoffBase.TotalMilliseconds;
        var maxMs = backoffMax.TotalMilliseconds;
        var delayMs = Math.Min(baseMs * Math.Pow(2, over - 1), maxMs);
        var delay = TimeSpan.FromMilliseconds(delayMs);

        _logger.LogWarning(
            $"Backpressure: {unprocessed} unprocessed files in inbox/{vehicleId}. Delaying next rotation by {delay}. Threshold={backlogThreshold}.");

        await Task.Delay(delay);
    }

    private static int CountUnprocessedFiles(string folder)
    {
        if (!Directory.Exists(folder)) return 0;

        return Directory.EnumerateFiles(folder, "telemetry_*").Select(Path.GetFileName)
            .Where(name => name is not null)
            .Count(name => name!.EndsWith(".jsonl", StringComparison.OrdinalIgnoreCase) ||
                           name!.EndsWith(".jsonl.gz", StringComparison.OrdinalIgnoreCase));
    }
}