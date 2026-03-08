namespace TCSA.KnowTheCity.Helpers;

using System.Text.RegularExpressions;
using TCSA.KnowTheCity.Models;

public static class CityData
{
    private static readonly Regex NonAlphanumericRegex = new("[^a-zA-Z0-9]", RegexOptions.Compiled);

    public static readonly List<CityInfo> Cities =
    [
        new()
        {
            Name = "Paris",
            Country = Country.France,
            Landmarks =
            [
                new() { Name = "Eiffel Tower" },
                new() { Name = "Louvre Museum" },
                new() { Name = "Notre-Dame" },
                new() { Name = "Arc de Triomphe" },
                new() { Name = "Sacré-Cœur" }
            ]
        },
        new()
        {
            Name = "London",
            Country = Country.UnitedKingdom,
            Landmarks =
            [
                new() { Name = "Big Ben" },
                new() { Name = "Tower Bridge" },
                new() { Name = "Buckingham Palace" },
                new() { Name = "London Eye" },
                new() { Name = "Tower of London" }
            ]
        },
        new()
        {
            Name = "New York",
            Country = Country.UnitedStates,
            Landmarks =
            [
                new() { Name = "Statue of Liberty" },
                new() { Name = "Empire State Building" },
                new() { Name = "Central Park" },
                new() { Name = "Brooklyn Bridge" },
                new() { Name = "Times Square" }
            ]
        },
        new()
        {
            Name = "Tokyo",
            Country = Country.Japan,
            Landmarks =
            [
                new() { Name = "Tokyo Tower" },
                new() { Name = "Senso-ji" },
                new() { Name = "Shibuya Crossing" },
                new() { Name = "Meiji Shrine" },
                new() { Name = "Tokyo Skytree" }
            ]
        },
        new()
        {
            Name = "Dubai",
            Country = Country.UnitedArabEmirates,
            Landmarks =
            [
                new() { Name = "Burj Khalifa" },
                new() { Name = "Palm Jumeirah" },
                new() { Name = "Dubai Mall" },
                new() { Name = "Burj Al Arab" },
                new() { Name = "Dubai Frame" }
            ]
        },
        new()
        {
            Name = "Beijing",
            Country = Country.China,
            Landmarks =
            [
                new() { Name = "Great Wall" },
                new() { Name = "Forbidden City" },
                new() { Name = "Temple of Heaven" },
                new() { Name = "Tiananmen Square" },
                new() { Name = "Summer Palace" }
            ]
        }
    ];

    public static List<string> GetLandmarks(string cityName) =>
        Cities.FirstOrDefault(c => c.Name == cityName)
            ?.Landmarks.Select(l => l.Name).ToList() ?? [];

    /// <summary>
    /// Builds an image path following the pattern: {city}-{landmark}.png
    /// where landmark is lowercased with spaces/special characters removed.
    /// Example: ("Paris", "Eiffel Tower") => "img/landmarks/paris-eiffeltower.png"
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