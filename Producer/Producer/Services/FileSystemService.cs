using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Producer.Services;

public class FileSystemService : IDisposable
{
    private readonly string _vehicleId;
    private readonly string _baseFolder;
    private readonly bool _useCompression;
    private readonly string _encoding = "utf8";
    private readonly string _version = "2.0";

    private readonly int _maxRecords;
    private readonly long _maxBytes;
    private readonly TimeSpan _maxAge;

    private int _recordCount;
    private long _bytesWritten;
    private DateTime _openedUtc;

    private FileStream? _fileStream;
    private StreamWriter? _writer;
    private string? _tmpPath;
    private string? _finalPath;

    public FileSystemService(
        string baseFolder,
        string vehicleId,
        bool compress = false,
        int maxRecords = 5000,
        long maxBytes = 10 * 1024 * 1024,
        TimeSpan? maxAge = null)
    {
        _baseFolder = baseFolder;
        _vehicleId = vehicleId;
        _useCompression = compress;
        _maxRecords = maxRecords;
        _maxBytes = maxBytes;
        _maxAge = maxAge ?? TimeSpan.FromMinutes(1);
    }

    // Open first temp file
    public void OpenNewFile()
    {
        Directory.CreateDirectory(Path.Combine(_baseFolder, _vehicleId));

        var ts = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var ext = _useCompression ? ".jsonl.gz" : ".jsonl";
        var fileName = $"telemetry_{ts}_{_vehicleId}{ext}";
        var folder = Path.Combine(_baseFolder, _vehicleId);
        _tmpPath = Path.Combine(folder, fileName + ".tmp");
        _finalPath = Path.Combine(folder, fileName);

        _fileStream = new FileStream(_tmpPath, FileMode.Create, FileAccess.Write, FileShare.None, 64 * 1024,
            useAsync: true);
        Stream dataStream = _useCompression
            ? new GZipStream(_fileStream, CompressionLevel.Fastest)
            : _fileStream;

        _writer = new StreamWriter(dataStream, new UTF8Encoding(false), bufferSize: 64 * 1024);
        _recordCount = 0;
        _bytesWritten = 0;
        _openedUtc = DateTime.UtcNow;
    }

    public async Task AddAsync(object record)
    {
        var prefix = ",\n";
        if (_writer == null)
        {
            OpenNewFile();
            await _writer!.WriteLineAsync("[");
            prefix = "";
        }

        var json = JsonSerializer.Serialize(record, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await _writer!.WriteAsync(prefix + json);

        _recordCount++;
        _bytesWritten += Encoding.UTF8.GetByteCount(json) + 1; // + newline

        if (ShouldRotate())
            await RotateAsync();
    }

    private bool ShouldRotate()
    {
        if (_recordCount >= _maxRecords) return true;
        if (_bytesWritten >= _maxBytes) return true;
        if (DateTime.UtcNow - _openedUtc >= _maxAge) return true;
        return false;
    }

    private async Task RotateAsync()
    {
        await _writer!.WriteLineAsync("\n]");

        await _writer!.FlushAsync();
        await _writer!.DisposeAsync();
        await _fileStream!.DisposeAsync();

        // Compute checksum
        string sha256;
        await using (var fs = File.OpenRead(_tmpPath!))
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(fs);
            sha256 = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        // Rename .tmp -> final
        File.Move(_tmpPath!, _finalPath!, overwrite: true);

        // Write metadata
        var meta = new
        {
            version = _version,
            createdUtc = DateTime.UtcNow.ToString("o"),
            recordCount = _recordCount,
            sha256,
            encoding = _encoding,
            compression = _useCompression ? "gzip" : "none"
        };

        var metaPath = _finalPath! + ".meta.json";
        await File.WriteAllTextAsync(metaPath,
            JsonSerializer.Serialize(meta, new JsonSerializerOptions { WriteIndented = true }));

        // Start new temp file
        OpenNewFile();
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