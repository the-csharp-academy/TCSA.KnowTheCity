using Microsoft.Extensions.Localization;

namespace TCSA.KnowTheCity.Localization;

public static class LocalizationExtensions
{
    public static string LocalizeEnum<TEnum>(this IStringLocalizer localizer, TEnum value)
        where TEnum : struct, Enum
    {
        var key = $"{typeof(TEnum).Name}.{value}";
        var localizedValue = localizer[key];

        return localizedValue.ResourceNotFound
            ? Humanize(value.ToString())
            : localizedValue.Value;
    }

    public static string Humanize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var builder = new System.Text.StringBuilder(value.Length + 8);
        builder.Append(value[0]);

        for (var i = 1; i < value.Length; i++)
        {
            if (char.IsUpper(value[i]) && !char.IsUpper(value[i - 1]))
            {
                builder.Append(' ');
            }

            builder.Append(value[i]);
        }

        return builder.ToString();
    }
}
