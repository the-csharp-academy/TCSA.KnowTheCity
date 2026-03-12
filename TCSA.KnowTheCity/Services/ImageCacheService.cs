using Microsoft.Extensions.Options;
using TCSA.KnowTheCity.Core.Options;
using TCSA.KnowTheCity.Core.Services;

namespace TCSA.KnowTheCity.Services;

public sealed class ImageCacheService(
    HttpClient httpClient,
    IOptions<ConfigOptions> options) : IImageCacheService
{
    private readonly string _cacheRoot = Path.Combine(FileSystem.CacheDirectory, "images");

    public async Task<string> GetImagePathAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var localPath = Path.Combine(_cacheRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(localPath))
            return localPath;

        var url = $"{options.Value.CdnUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";

        try
        {
            Console.WriteLine($"[ImageCacheService] Requesting: {url}");
            var responseBytes = await httpClient.GetByteArrayAsync(url, cancellationToken);

            var directory = Path.GetDirectoryName(localPath)!;
            Directory.CreateDirectory(directory);

            await File.WriteAllBytesAsync(localPath, responseBytes, cancellationToken);

            return localPath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ImageCacheService] Failed to download: {url}");
            Console.WriteLine($"[ImageCacheService] Exception: {ex}");
            throw;
        }
    }
}