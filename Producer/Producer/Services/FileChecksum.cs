using System.Security.Cryptography;

namespace Producer.Services;

public class FileChecksum
{
    public async Task<string> GetChecksum(string filePath)
    {
        await using var fs = File.OpenRead(filePath);
        using var sha = SHA256.Create();

        var hash = await sha.ComputeHashAsync(fs).ConfigureAwait(false);
        var sha256 = Convert.ToHexString(hash).ToLowerInvariant();

        return sha256;
    }
}