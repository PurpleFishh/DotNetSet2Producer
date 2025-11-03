using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Producer.Entity;

namespace Producer.Services;

public class FileSystemService(
    string baseFolder,
    string vehicleId,
    CompressionKind compress = CompressionKind.None,
    int maxRecords = 5000,
    long maxBytes = 10 * 1024 * 1024,
    TimeSpan? maxAge = null)
    : IDisposable
{
    private const string Version = "2.0";

    private readonly TimeSpan _maxAge = maxAge ?? TimeSpan.FromMinutes(1);

    private int _recordCount;
    private long _bytesWritten;
    private DateTime _openedUtc;

    private FileStream? _fileStream;
    private StreamWriter? _writer;
    private string? _tmpPath;
    private string? _finalPath;

    private readonly FileMetadataService _metadataService = new FileMetadataService();

    private void OpenNewFile()
    {
        Directory.CreateDirectory(Path.Combine(baseFolder, vehicleId));

        var ts = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var ext = compress == CompressionKind.Gzip ? ".jsonl.gz" : ".jsonl";
        var fileName = $"telemetry_{ts}_{vehicleId}{ext}";
        var folder = Path.Combine(baseFolder, vehicleId);
        _tmpPath = Path.Combine(folder, fileName + ".tmp");
        _finalPath = Path.Combine(folder, fileName);

        _fileStream = new FileStream(_tmpPath, FileMode.Create, FileAccess.Write, FileShare.None, 64 * 1024,
            useAsync: true);
        Stream dataStream = compress == CompressionKind.Gzip
            ? new GZipStream(_fileStream, CompressionLevel.Fastest)
            : _fileStream;

        _writer = new StreamWriter(dataStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            bufferSize: 64 * 1024);
        _writer.NewLine = "\n";

        _recordCount = 0;
        _bytesWritten = 0;
        _openedUtc = DateTime.UtcNow;
    }

    public async Task AddAsync(object record)
    {
        var first = false;
        if (_writer == null)
        {
            OpenNewFile();
            await _writer!.WriteLineAsync("[");
            first = true;
        }

        var json = JsonSerializer.Serialize(record, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var prefix = first ? "" : $",{_writer.NewLine}";
        await _writer!.WriteAsync(prefix + json);

        _recordCount++;
        _bytesWritten += Encoding.UTF8.GetByteCount(json);

        if (ShouldRotate())
            await RotateAsync();
    }

    private bool ShouldRotate()
    {
        if (_recordCount >= maxRecords) return true;
        if (_bytesWritten >= maxBytes) return true;
        if (DateTime.UtcNow - _openedUtc >= _maxAge) return true;
        return false;
    }

    private async Task RotateAsync()
    {
        await _writer!.WriteLineAsync($"{_writer.NewLine}]");

        await _writer!.FlushAsync();
        await _writer!.DisposeAsync();
        await _fileStream!.DisposeAsync();
        _writer = null;

        var sha256 = await new FileChecksum().GetChecksum(_tmpPath!);
        File.Move(_tmpPath!, _finalPath!, overwrite: true);
        await _metadataService.WriteMetaDataFileForFinal(_finalPath!, Version, _recordCount, compress, sha256);
    }

    public async ValueTask DisposeAsync()
    {
        if (_writer != null)
        {
            await _writer.FlushAsync();
            await _writer.DisposeAsync();
        }

        _fileStream?.Dispose();
    }

    public void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();
}