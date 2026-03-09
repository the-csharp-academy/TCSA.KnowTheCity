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
            Id = "paris-fr",
            Name = "Paris",
            Country = Country.France,
            Landmarks =
            [
                new() { Id = "eiffel-tower",    Name = "Eiffel Tower" },
                new() { Id = "louvre-museum",   Name = "Louvre Museum" },
                new() { Id = "notre-dame",      Name = "Notre-Dame" },
                new() { Id = "arc-de-triomphe", Name = "Arc de Triomphe" },
                new() { Id = "sacre-coeur",     Name = "Sacré-Cœur" }
            ]
        },
        new()
        {
            Id = "london-uk",
            Name = "London",
            Country = Country.UnitedKingdom,
            Landmarks =
            [
                new() { Id = "big-ben",            Name = "Big Ben" },
                new() { Id = "tower-bridge",       Name = "Tower Bridge" },
                new() { Id = "buckingham-palace",  Name = "Buckingham Palace" },
                new() { Id = "london-eye",         Name = "London Eye" },
                new() { Id = "tower-of-london",    Name = "Tower of London" }
            ]
        },
        new()
        {
            Id = "newyork-us",
            Name = "New York",
            Country = Country.UnitedStates,
            Landmarks =
            [
                new() { Id = "statue-of-liberty",    Name = "Statue of Liberty" },
                new() { Id = "empire-state-building", Name = "Empire State Building" },
                new() { Id = "central-park",         Name = "Central Park" },
                new() { Id = "brooklyn-bridge",      Name = "Brooklyn Bridge" },
                new() { Id = "times-square",         Name = "Times Square" }
            ]
        },
        new()
        {
            Id = "tokyo-jp",
            Name = "Tokyo",
            Country = Country.Japan,
            Landmarks =
            [
                new() { Id = "tokyo-tower",    Name = "Tokyo Tower" },
                new() { Id = "senso-ji",       Name = "Senso-ji" },
                new() { Id = "shibuya-crossing", Name = "Shibuya Crossing" },
                new() { Id = "meiji-shrine",   Name = "Meiji Shrine" },
                new() { Id = "tokyo-skytree",  Name = "Tokyo Skytree" }
            ]
        },
        new()
        {
            Id = "dubai-uae",
            Name = "Dubai",
            Country = Country.UnitedArabEmirates,
            Landmarks =
            [
                new() { Id = "burj-khalifa",  Name = "Burj Khalifa" },
                new() { Id = "palm-jumeirah", Name = "Palm Jumeirah" },
                new() { Id = "dubai-mall",    Name = "Dubai Mall" },
                new() { Id = "burj-al-arab",  Name = "Burj Al Arab" },
                new() { Id = "dubai-frame",   Name = "Dubai Frame" }
            ]
        },
        new()
        {
            Id = "beijing-ch",
            Name = "Beijing",
            Country = Country.China,
            Landmarks =
            [
                new() { Id = "great-wall",       Name = "Great Wall" },
                new() { Id = "forbidden-city",   Name = "Forbidden City" },
                new() { Id = "temple-of-heaven", Name = "Temple of Heaven" },
                new() { Id = "tiananmen-square", Name = "Tiananmen Square" },
                new() { Id = "summer-palace",    Name = "Summer Palace" }
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