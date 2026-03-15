using MudBlazor;

namespace TCSA.KnowTheCity.Theme;

public static class AppTheme
{
    public static MudTheme Default { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#11145F",
            Secondary = "#0F6CC7",
            Tertiary = "#F9DD1F",
            Info = "#2BB7F1",
            Success = "#0F6CC7",
            Warning = "#F49A12",
            Error = "#E84A1C",
            Background = "#F5E8C9",
            Surface = "#FFFFFF",
            AppbarBackground = "#11145F",
            AppbarText = "#F5E8C9",
            TextPrimary = "#11145F",
            TextSecondary = "#0F6CC7"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#2BB7F1",
            Secondary = "#0F6CC7",
            Tertiary = "#F9DD1F",
            Info = "#2BB7F1",
            Success = "#0F6CC7",
            Warning = "#F49A12",
            Error = "#E84A1C",
            Background = "#11145F",
            Surface = "#0F6CC7",
            AppbarBackground = "#11145F",
            AppbarText = "#F5E8C9",
            TextPrimary = "#F5E8C9",
            TextSecondary = "#2BB7F1"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px"
        }
    };
}