using System.Globalization;

namespace TCSA.KnowTheCity.Localization;

public sealed class AppLocalizationService
{
    private const string CulturePreferenceKey = "app-culture";
    private const string DefaultCultureName = "en-US";
    private string _currentCultureName = DefaultCultureName;

    private readonly IReadOnlyList<SupportedCultureOption> _supportedCultures =
    [
        new("en-US", "English"),
        new("es-ES", "Español"),
        new("pt-BR", "Português (Brasil)"),
        new("it-IT", "Italiano"),
        new("fr-FR", "Français"),
        new("de-DE", "Deutsch")
    ];

    public event Action? CultureChanged;

    public IReadOnlyList<SupportedCultureOption> SupportedCultures => _supportedCultures;

    public string CurrentCultureName => _currentCultureName;

    public void Initialize()
    {
        var initialCulture = GetInitialCultureName();

        if (MainThread.IsMainThread)
        {
            ApplyCulture(initialCulture, persistPreference: false);
            return;
        }

        MainThread.BeginInvokeOnMainThread(() => ApplyCulture(initialCulture, persistPreference: false));
    }

    public async Task SetCultureAsync(string? cultureName)
    {
        if (MainThread.IsMainThread)
        {
            ApplyCulture(cultureName, persistPreference: true);
            return;
        }

        await MainThread.InvokeOnMainThreadAsync(() => ApplyCulture(cultureName, persistPreference: true));
    }

    private string GetInitialCultureName()
    {
        var savedCulture = Preferences.Default.Get(CulturePreferenceKey, string.Empty);
        if (IsSupportedCulture(savedCulture))
        {
            return savedCulture;
        }

        var deviceCulture = CultureInfo.CurrentUICulture;
        var matchedCulture = _supportedCultures.FirstOrDefault(culture =>
            string.Equals(culture.Culture.TwoLetterISOLanguageName, deviceCulture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase));

        return matchedCulture?.Name ?? DefaultCultureName;
    }

    private void ApplyCulture(string? cultureName, bool persistPreference)
    {
        var selectedCultureName = IsSupportedCulture(cultureName)
            ? cultureName!
            : DefaultCultureName;

        var culture = CultureInfo.GetCultureInfo(selectedCultureName);

        _currentCultureName = culture.Name;

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        if (persistPreference)
        {
            Preferences.Default.Set(CulturePreferenceKey, culture.Name);
        }

        CultureChanged?.Invoke();
    }

    private bool IsSupportedCulture(string? cultureName)
    {
        return _supportedCultures.Any(culture =>
            string.Equals(culture.Name, cultureName, StringComparison.OrdinalIgnoreCase));
    }
}

public sealed record SupportedCultureOption(string Name, string DisplayName)
{
    public CultureInfo Culture => CultureInfo.GetCultureInfo(Name);
}
