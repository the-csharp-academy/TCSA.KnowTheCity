namespace TCSA.KnowTheCity.Core.Helpers;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TCSA.KnowTheCity.Models;

public static class CityDataHelper
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
    /// Example: ("Paris", "Sacré-Cœur")   => "img/landmarks/paris-sacrecoeur.png"
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

    public static string NormalizeForPath(string value)
    {
        // Step 1: handle ligatures and multi-char equivalents that NFD cannot decompose
        var expanded = value
            .Replace("œ", "oe", StringComparison.Ordinal)
            .Replace("Œ", "oe", StringComparison.Ordinal)
            .Replace("æ", "ae", StringComparison.Ordinal)
            .Replace("Æ", "ae", StringComparison.Ordinal)
            .Replace("ß", "ss", StringComparison.Ordinal);

        // Step 2: NFD decomposition separates base letters from diacritics (é -> e + ?)
        var normalized = expanded.Normalize(NormalizationForm.FormD);

        // Step 3: keep only ASCII base letters — diacritic combining marks are NonSpacingMark category
        var asciiOnly = new StringBuilder(normalized.Length);
        foreach (var ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                asciiOnly.Append(ch);
        }

        // Step 4: strip anything that is still not alphanumeric, then lowercase
        var alphanumericOnly = NonAlphanumericRegex.Replace(asciiOnly.ToString(), "");
        return alphanumericOnly.ToLowerInvariant();
    }
}