namespace TCSA.KnowTheCity.Core.Helpers;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TCSA.KnowTheCity.Core.Enums;
using TCSA.KnowTheCity.Core.Models.Domain;

public static class CityDataHelper
{
    private static readonly Regex NonAlphanumericRegex = new("[^a-zA-Z0-9]", RegexOptions.Compiled);

    public static readonly List<City> SeedData =
    [
        new()
        {
            Name = "Paris",
            Country = Country.France,
            Continent = Continent.Europe,
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
            Continent = Continent.Europe,
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
            Continent = Continent.NorthAmerica,
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
            Continent = Continent.Asia,
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
            Continent = Continent.Asia,
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
            Continent = Continent.Asia,
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

    public static string GetLandmarkImagePath(string cityName, string landmarkName) => 
        $"landmarks/{NormalizeForPath(cityName)}-{NormalizeForPath(landmarkName)}.webp";
        
    public static string GetCityImagePath(string cityName) =>
        $"cities/{NormalizeForPath(cityName)}.webp";

    public static string NormalizeForPath(string value)
    {
        var expanded = value
            .Replace("œ", "oe", StringComparison.Ordinal)
            .Replace("Œ", "oe", StringComparison.Ordinal)
            .Replace("æ", "ae", StringComparison.Ordinal)
            .Replace("Æ", "ae", StringComparison.Ordinal)
            .Replace("ß", "ss", StringComparison.Ordinal);

        var normalized = expanded.Normalize(NormalizationForm.FormD);

        var asciiOnly = new StringBuilder(normalized.Length);
        foreach (var ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                asciiOnly.Append(ch);
        }

        var alphanumericOnly = NonAlphanumericRegex.Replace(asciiOnly.ToString(), "");
        return alphanumericOnly.ToLowerInvariant();
    }
}