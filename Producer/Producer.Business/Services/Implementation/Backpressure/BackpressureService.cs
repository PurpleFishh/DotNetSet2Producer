using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Producer.Business.Services.Interface;
using Producer.Common;
using Producer.Common.Config;

namespace Producer.Business.Services.Implementation.Backpressure;

public class BackpressureService(
    IVehicleContext vehicle,
    IOptions<PathOptions> paths,
    IOptions<WindowSizeOptions> bp,
    ILogger<BackpressureService> logger)
    : IBackpressureService
{
    private readonly string _baseFolder = paths.Value.BaseFolder;
    private readonly string _vehicleId = vehicle.VehicleId;

    private readonly int _backlogThreshold = bp.Value.BackpressureMaxUnprocessed;
    private readonly TimeSpan _backoffBase = TimeSpan.FromSeconds(bp.Value.BackoffBaseSeconds);
    private readonly TimeSpan _backoffMax = TimeSpan.FromSeconds(bp.Value.BackoffMaxSeconds);

    public async Task ApplyBackpressureAsync()
    {
        var unprocessed = CountUnprocessedFiles(_baseFolder);

        if (unprocessed <= _backlogThreshold)
            return;

        var over = Math.Clamp(unprocessed - _backlogThreshold, 1, 8);
        var baseMs = _backoffBase.TotalMilliseconds;
        var maxMs = _backoffMax.TotalMilliseconds;
        var delayMs = Math.Min(baseMs * Math.Pow(2, over - 1), maxMs);
        var delay = TimeSpan.FromMilliseconds(delayMs);

        logger.LogWarning(
            $"Backpressure: {unprocessed} unprocessed files in inbox/{_vehicleId}. Delaying next rotation by {delay}. Threshold={_backlogThreshold}.");

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