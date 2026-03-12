namespace TCSA.KnowTheCity.Core.Helpers;

public static class ImageHelpers
{
    public static string ConvertToBase64DataUri(string localPath)
    {
        var bytes = File.ReadAllBytes(localPath);
        var extension = Path.GetExtension(localPath).ToLowerInvariant();
        var mimeType = extension switch
        {
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "image/jpeg"
        };
        return $"data:{mimeType};base64,{Convert.ToBase64String(bytes)}";
    }
}
