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
    public string ManifestPath { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class CityDetailsManifest
{
    public string CityRemoteId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public List<MonumentManifestItem> Monuments { get; set; } = new();
}

public sealed class MonumentManifestItem
{
    public string RemoteId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string ImageVersion { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
