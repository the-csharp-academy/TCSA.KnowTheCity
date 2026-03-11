using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using TCSA.KnowTheCity.Core.Models.DTOs;
using TCSA.KnowTheCity.Core.Options;

namespace TCSA.KnowTheCity.Core.Clients;

public interface IManifestClient
{
    Task<CitiesManifest?> GetCitiesManifestAsync(CancellationToken cancellationToken = default);
    Task<CityDetailsManifest?> GetCityManifestAsync(string manifestPath, CancellationToken cancellationToken = default);
}

public sealed class ManifestClient(
    HttpClient _httpClient,
    IOptions<ConfigOptions> config
    ) : IManifestClient
{
    public Task<CitiesManifest?> GetCitiesManifestAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{config.Value.CdnUrl.TrimEnd('/')}/manifests/cities.json";
        return _httpClient.GetFromJsonAsync<CitiesManifest>(url, cancellationToken);
    }

    public Task<CityDetailsManifest?> GetCityManifestAsync(string manifestPath, CancellationToken cancellationToken = default)
    {
        var url = $"{config.Value.CdnUrl.TrimEnd('/')}/{manifestPath.TrimStart('/')}";
        return _httpClient.GetFromJsonAsync<CityDetailsManifest>(url, cancellationToken);
    }
}