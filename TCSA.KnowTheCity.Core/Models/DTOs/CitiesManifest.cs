namespace TCSA.KnowTheCity.Core.Models.DTOs;

public sealed class CitiesManifest
{
    public string Version { get; set; } = string.Empty;
    public List<CityManifestItem> Cities { get; set; } = new();
}

public sealed class CityManifestItem
{
    public string RemoteId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Continent { get; set; } = string.Empty;
    public string ManifestPath { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public DateTime? DateAdded { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class CityDetailsManifest
{
    public string CityRemoteId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public List<LocalizedNameManifestItem> CityNames { get; set; } = new();
    public List<MonumentManifestItem> Monuments { get; set; } = new();
}

public sealed class MonumentManifestItem
{
    public string RemoteId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string MobileImagePath { get; set; } = string.Empty;
    public string ImageVersion { get; set; } = string.Empty;
    public DateTime? DateAdded { get; set; }
    public bool IsActive { get; set; } = true;
    public List<LocalizedNameManifestItem> Names { get; set; } = new();
}

public sealed class LocalizedNameManifestItem
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
