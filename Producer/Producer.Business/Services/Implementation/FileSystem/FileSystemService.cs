using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Producer.Business.Services.Interface;
using Producer.Common;
using Producer.Common.Config;
using Producer.Common.Extensions;
using Producer.Common.Types;

namespace Producer.Business.Services.Implementation.FileSystem;

public class FileSystemService : IFileSystemService
{
    private readonly string _vehicleId;
    private readonly DataSchemas _version;

    private readonly FileSystemServiceOptions _fsOptions;

    private int _recordCount;
    private long _bytesWritten;
    private DateTime _openedUtc;

    private FileStream? _fileStream;
    private StreamWriter? _writer;
    private string? _tmpPath;
    private string? _finalPath;

    private readonly IFileMetadataService _metadataService;
    private readonly IBackpressureService _backpressureService;
    private readonly IFaultInjectionService _faultInjection;
    private readonly ILogger<FileSystemService> _logger;

    public FileSystemService(
        IOptions<ProducerOptions> optionsAccessor,
        IVehicleContext vehicle,
        IBackpressureService backpressureService,
        IFaultInjectionService faultInjection,
        IFileMetadataService metadataService,
        ILogger<FileSystemService> logger)
    {
        _fsOptions = FileSystemServiceOptions.From(optionsAccessor.Value);
        _vehicleId = vehicle.VehicleId;
        _version = vehicle.Version;

        _logger = logger;
        _backpressureService = backpressureService;
        _faultInjection = faultInjection;
        _metadataService = metadataService;
    }

    private void OpenNewFile()
    {
        Directory.CreateDirectory(_fsOptions.BaseFolder);

        var ts = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var ext = _fsOptions.FileCompression == FileCompressionType.Gzip ? ".jsonl.gz" : ".jsonl";
        var fileName = $"telemetry_{ts}_{_vehicleId}{ext}";
        _tmpPath = Path.Combine(_fsOptions.BaseFolder, fileName + ".tmp");
        _finalPath = Path.Combine(_fsOptions.BaseFolder, fileName);

        _logger.LogInformation(
            "Opening new telemetry file. VehicleId={VehicleId} TmpPath={TmpPath} FinalPath={FinalPath} Compression={Compression} MaxRecords={MaxRecords} MaxBytes={MaxBytes} MaxAgeSeconds={MaxAgeSeconds}",
            _vehicleId, _tmpPath, _finalPath, _fsOptions.FileCompression, _fsOptions.MaxRecords, _fsOptions.MaxBytes,
            _fsOptions.MaxAge.TotalSeconds);

        _fileStream = new FileStream(_tmpPath, FileMode.Create, FileAccess.Write, FileShare.None, 64 * 1024,
            useAsync: true);
        Stream dataStream = _fsOptions.FileCompression == FileCompressionType.Gzip
            ? new GZipStream(_fileStream, CompressionLevel.Fastest)
            : _fileStream;

        _writer = new StreamWriter(dataStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            bufferSize: 64 * 1024);
        _writer.NewLine = "\n";

        _recordCount = 0;
        _bytesWritten = 0;
        _openedUtc = DateTime.UtcNow;
        _logger.LogDebug("Telemetry file opened at {OpenedUtc:O}", _openedUtc);
    }

    public async Task AddAsync(object record)
    {
        if (_faultInjection.ShouldDropRecord())
        {
            _logger.LogInformation("Dropping record due to fault injection");
            return;
        }

        var first = false;
        if (_writer == null)
        {
            OpenNewFile();
            await _writer!.WriteLineAsync("[");
            first = true;
            _logger.LogDebug("Started JSON array for new telemetry file {TmpPath}", _tmpPath);
        }

        var json = JsonSerializer.Serialize(record, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var prefix = first ? "" : $",{_writer.NewLine}";
        await _writer!.WriteAsync(prefix + json);

        _recordCount++;
        _bytesWritten += Encoding.UTF8.GetByteCount(json);

        if (_recordCount == 1 || _recordCount % 100 == 0)
            _logger.LogDebug(
                "Record appended. Count={RecordCount} BytesWritten={BytesWritten} AgeSeconds={AgeSeconds:F2}",
                _recordCount, _bytesWritten, (DateTime.UtcNow - _openedUtc).TotalSeconds);


        if (ShouldRotate())
            await RotateAsync();
    }

    private bool ShouldRotate()
    {
        if (_recordCount >= _fsOptions.MaxRecords)
        {
            _logger.LogInformation(
                "Rotating file due to maxRecords reached. Count={RecordCount} MaxRecords={MaxRecords}", _recordCount,
                _fsOptions.MaxRecords);
            return true;
        }

        if (_bytesWritten >= _fsOptions.MaxBytes)
        {
            _logger.LogInformation(
                "Rotating file due to maxBytes reached. BytesWritten={BytesWritten} MaxBytes={MaxBytes}", _bytesWritten,
                _fsOptions.MaxBytes);
            return true;
        }

        var age = DateTime.UtcNow - _openedUtc;
        if (age >= _fsOptions.MaxAge)
        {
            _logger.LogInformation(
                "Rotating file due to maxAge reached. AgeSeconds={AgeSeconds:F2} MaxAgeSeconds={MaxAgeSeconds:F2}",
                age.TotalSeconds, _fsOptions.MaxAge.TotalSeconds);
            return true;
        }

        return false;
    }

    private async Task RotateAsync()
    {
        _logger.LogInformation(
            "Rotating telemetry file. TmpPath={TmpPath} FinalPath={FinalPath} Records={RecordCount} Bytes={BytesWritten} AgeSeconds={AgeSeconds:F2}",
            _tmpPath, _finalPath, _recordCount, _bytesWritten, (DateTime.UtcNow - _openedUtc).TotalSeconds);

        await _writer!.WriteLineAsync($"{_writer.NewLine}]");

        await _writer!.FlushAsync();
        await _writer!.DisposeAsync();
        await _fileStream!.DisposeAsync();
        _writer = null;
        _logger.LogDebug("Writer and stream disposed for {TmpPath}", _tmpPath);

        await FinalizeFile();
        await _backpressureService.ApplyBackpressureAsync();
        _logger.LogDebug("Backpressure applied after rotation for vehicle {VehicleId}", _vehicleId);
    }

    private async Task FinalizeFile()
    {
        _logger.LogDebug("Finalizing file. TmpPath={TmpPath} FinalPath={FinalPath}", _tmpPath, _finalPath);
        var corruptionPhase = _faultInjection.GetRandomPhase();
        if (corruptionPhase == FaultPhaseType.BeforeHash)
        {
            _logger.LogWarning("Potential corruption before hash on {TmpPath}", _tmpPath);
            _faultInjection.MaybeCorruptTail(_tmpPath!);
        }

        var sha256 = await new FileChecksum().GetChecksum(_tmpPath!);
        _logger.LogDebug("Checksum computed for {TmpPath} Sha256={Sha256}", _tmpPath, sha256);
        await _metadataService.WriteMetaDataFileForFinal(_tmpPath!, _version, _recordCount, _fsOptions.FileCompression,
            sha256);
        File.Move(_tmpPath!, _finalPath!, overwrite: true);
        _logger.LogInformation(
            "Moved temp file to final. FinalPath={FinalPath} Records={RecordCount} Compression={Compression}",
            _finalPath, _recordCount, _fsOptions.FileCompression);
        _logger.LogDebug("Metadata file written for {FinalPath}", _finalPath);

        if (corruptionPhase == FaultPhaseType.AfterHash)
        {
            _logger.LogWarning("Potential corruption after hash on {FinalPath}", _finalPath);
            _faultInjection.MaybeCorruptTail(_finalPath!);
        }
    }

    private static bool IsFileContentFinalized(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return fs.GetLastChar() == ']';
    }


    public async Task FinalizeOnShutdownAsync()
    {
        if (_tmpPath is null || _finalPath is null)
        {
            _logger.LogDebug("FinalizeOnShutdownAsync skipped: no active file paths");
            return;
        }

        if (_writer != null)
        {
            _logger.LogInformation("FinalizeOnShutdownAsync: active writer detected, rotating file");
            await RotateAsync();
            return;
        }

        if (!File.Exists(_tmpPath))
        {
            _logger.LogDebug("FinalizeOnShutdownAsync: tmp file missing {TmpPath}", _tmpPath);
            return;
        }

        if (!IsFileContentFinalized(_tmpPath))
        {
            _logger.LogDebug("FinalizeOnShutdownAsync: appending closing bracket to {TmpPath}", _tmpPath);
            await File.AppendAllTextAsync(_tmpPath, "]", new UTF8Encoding(false));
        }

        await FinalizeFile();
        _logger.LogInformation("FinalizeOnShutdownAsync completed for {FinalPath}", _finalPath);
    }

    public void FinalizeOnShutdownSync() => FinalizeOnShutdownAsync().GetAwaiter().GetResult();

    public async ValueTask DisposeAsync()
    {
        _logger.LogDebug("DisposeAsync starting for vehicle {VehicleId}", _vehicleId);
        try
        {
            await FinalizeOnShutdownAsync();
        }
        finally
        {
            if (_writer != null)
            {
                _logger.LogDebug("DisposeAsync: flushing and disposing writer for {TmpPath}", _tmpPath);
                await _writer.FlushAsync();
                await _writer.DisposeAsync();
            }

            _fileStream?.Dispose();
            _logger.LogDebug("DisposeAsync completed for vehicle {VehicleId}", _vehicleId);
        }
    }

    public void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();
}