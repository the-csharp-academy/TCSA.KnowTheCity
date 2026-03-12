namespace TCSA.KnowTheCity.Core.Services;

public interface IImageCacheService
{
    /// <summary>
    /// Returns a local file path for the image. Downloads from the CDN
    /// on first access, then serves from cache on subsequent calls.
    /// </summary>
    Task<string> GetImagePathAsync(string relativePath, CancellationToken cancellationToken = default);
}