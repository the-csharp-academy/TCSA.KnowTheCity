namespace TCSA.KnowTheCity.Helpers;

using System.Text.RegularExpressions;

public record CityInfo(string Name, List<string> Landmarks);

public static class CityData
{
    private static readonly Regex NonAlphanumericRegex = new("[^a-zA-Z0-9]", RegexOptions.Compiled);

    public static readonly List<CityInfo> Cities =
    [
        new("Paris", ["Eiffel Tower", "Louvre Museum", "Notre-Dame", "Arc de Triomphe", "Sacré-Cœur"]),
        new("London", ["Big Ben", "Tower Bridge", "Buckingham Palace", "London Eye", "Tower of London"]),
        new("New York", ["Statue of Liberty", "Empire State Building", "Central Park", "Brooklyn Bridge", "Times Square"]),
        new("Tokyo", ["Tokyo Tower", "Senso-ji", "Shibuya Crossing", "Meiji Shrine", "Tokyo Skytree"]),
        new("Dubai", ["Burj Khalifa", "Palm Jumeirah", "Dubai Mall", "Burj Al Arab", "Dubai Frame"]),
        new("Beijing", ["Great Wall", "Forbidden City", "Temple of Heaven", "Tiananmen Square", "Summer Palace"])
    ];

    public static List<string> GetLandmarks(string cityName) =>
        Cities.FirstOrDefault(c => c.Name == cityName)?.Landmarks ?? [];

    /// <summary>
    /// Builds an image path following the pattern: {city}-{landmark}.png
    /// where landmark is lowercased with spaces/special characters removed.
    /// Example: ("Paris", "Eiffel Tower") => "images/paris-eiffeltower.png"
    /// </summary>
    public static string GetLandmarkImagePath(string cityName, string landmarkName)
    {
        var city = NormalizeForPath(cityName);
        var landmark = NormalizeForPath(landmarkName);
        return $"img/landmarks/{city}-{landmark}.png";
    }

    public static string GetCityImagePath(string cityName)
    {
        var city = NormalizeForPath(cityName);
        return $"img/cities/{city}.png";
    }

    private static string NormalizeForPath(string value)
    {
        var alphanumericOnly = NonAlphanumericRegex.Replace(value, "");
        return alphanumericOnly.ToLowerInvariant();
    }
}