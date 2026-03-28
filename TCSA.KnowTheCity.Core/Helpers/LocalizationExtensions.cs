using System.Globalization;
using TCSA.KnowTheCity.Core.Models.Domain;

namespace TCSA.KnowTheCity.Core.Helpers;

public static class LocalizationExtensions
{
    public static string GetLocalizedName(this City city) =>
        GetLocalizedName(city, CultureInfo.CurrentUICulture);

    public static string GetLocalizedName(this City city, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(city);
        ArgumentNullException.ThrowIfNull(culture);

        return GetLocalizedNameInternal(
            city.Name,
            city.Translations,
            culture);
    }

    public static string GetLocalizedName(this Landmark landmark) =>
        GetLocalizedName(landmark, CultureInfo.CurrentUICulture);

    public static string GetLocalizedName(this Landmark landmark, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(landmark);
        ArgumentNullException.ThrowIfNull(culture);

        return GetLocalizedNameInternal(
            landmark.Name,
            landmark.Translations,
            culture);
    }

    private static string GetLocalizedNameInternal<TTranslation>(
        string fallbackName,
        IEnumerable<TTranslation>? translations,
        CultureInfo culture)
    {
        if (translations is null)
        {
            return fallbackName;
        }

        var exactMatch = string.Empty;
        var languageMatch = string.Empty;
        var currentCultureName = culture.Name;
        var currentLanguage = culture.TwoLetterISOLanguageName;

        foreach (var translation in translations)
        {
            if (translation is null)
            {
                continue;
            }

            var languageCode = GetLanguageCode(translation);
            var translatedName = GetName(translation);

            if (string.IsNullOrWhiteSpace(languageCode) || string.IsNullOrWhiteSpace(translatedName))
            {
                continue;
            }

            if (languageCode.Equals(currentCultureName, StringComparison.OrdinalIgnoreCase))
            {
                exactMatch = translatedName;
                break;
            }

            if (string.IsNullOrEmpty(languageMatch) && languageCode.StartsWith(currentLanguage, StringComparison.OrdinalIgnoreCase))
            {
                languageMatch = translatedName;
            }
        }

        if (!string.IsNullOrWhiteSpace(exactMatch))
        {
            return exactMatch;
        }

        if (!string.IsNullOrWhiteSpace(languageMatch))
        {
            return languageMatch;
        }

        return fallbackName;
    }

    private static string? GetLanguageCode<TTranslation>(TTranslation translation) => translation switch
    {
        CityTranslation cityTranslation => cityTranslation.LanguageCode,
        LandmarkTranslation landmarkTranslation => landmarkTranslation.LanguageCode,
        _ => null
    };

    private static string? GetName<TTranslation>(TTranslation translation) => translation switch
    {
        CityTranslation cityTranslation => cityTranslation.Name,
        LandmarkTranslation landmarkTranslation => landmarkTranslation.Name,
        _ => null
    };
}
