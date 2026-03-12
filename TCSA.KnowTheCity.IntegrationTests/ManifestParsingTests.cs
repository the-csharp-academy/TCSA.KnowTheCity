using Microsoft.Extensions.Options;
using TCSA.KnowTheCity.Core.Clients;
using TCSA.KnowTheCity.Core.Models.DTOs;
using TCSA.KnowTheCity.Core.Options;

namespace TCSA.KnowTheCity.IntegrationTests;

public class ManifestParsingTests
{
    private const string BaseCdnUrl = "https://mock-cdn.test";
    private const string FixturesPath = "Fixtures";

    private IManifestClient _manifestClient = default!;
    private CitiesManifest _citiesManifest = default!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var routes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [$"{BaseCdnUrl}/manifests/cities.json"] = await LoadFixtureAsync("cities.json"),
            [$"{BaseCdnUrl}/manifests/paris.json"]  = await LoadFixtureAsync("paris.json"),
            [$"{BaseCdnUrl}/manifests/london.json"] = await LoadFixtureAsync("london.json"),
        };

        var handler = new MockCdnHandler(routes);
        var httpClient = new HttpClient(handler);
        var options = Options.Create(new ConfigOptions { CdnUrl = BaseCdnUrl });

        _manifestClient = new ManifestClient(httpClient, options);
        _citiesManifest = (await _manifestClient.GetCitiesManifestAsync())!;
    }

    private static Task<string> LoadFixtureAsync(string fileName)
    {
        var path = Path.Combine(FixturesPath, fileName);
        return File.ReadAllTextAsync(path);
    }

    [Test]
    public void CitiesManifest_IsNotNull()
    {
        Assert.That(_citiesManifest, Is.Not.Null);
    }

    [Test]
    public void CitiesManifest_HasVersion()
    {
        Assert.That(_citiesManifest.Version, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void CitiesManifest_HasAtLeastOneCity()
    {
        Assert.That(_citiesManifest.Cities, Is.Not.Empty);
    }

    [Test]
    public void CitiesManifest_EachCity_HasRequiredFields()
    {
        Assert.Multiple(() =>
        {
            foreach (var city in _citiesManifest.Cities)
            {
                Assert.That(city.RemoteId, Is.Not.Null.And.Not.Empty,
                    $"City '{city.Name}' is missing RemoteId");
                Assert.That(city.Name, Is.Not.Null.And.Not.Empty,
                    $"City with RemoteId '{city.RemoteId}' is missing Name");
                Assert.That(city.Country, Is.Not.Null.And.Not.Empty,
                    $"City '{city.Name}' is missing Country");
                Assert.That(city.ManifestPath, Is.Not.Null.And.Not.Empty,
                    $"City '{city.Name}' is missing ManifestPath");
            }
        });
    }

    [Test]
    public void CitiesManifest_EachCity_CountryMapsToKnownEnum()
    {
        Assert.Multiple(() =>
        {
            foreach (var city in _citiesManifest.Cities)
            {
                Assert.That(
                    Enum.TryParse<Core.Enums.Country>(city.Country, ignoreCase: true, out _),
                    Is.True,
                    $"Country '{city.Country}' for city '{city.Name}' does not map to a known Country enum value");
            }
        });
    }

    // --- CityDetailsManifest ---

    [Test]
    public async Task CityDetailsManifest_IsNotNull_ForEachCity()
    {
        foreach (var city in _citiesManifest.Cities)
        {
            var details = await _manifestClient.GetCityManifestAsync(city.ManifestPath);
            Assert.That(details, Is.Not.Null, $"Details manifest for city '{city.Name}' returned null");
        }
    }

    [Test]
    public async Task CityDetailsManifest_HasMatchingRemoteId_ForEachCity()
    {
        foreach (var city in _citiesManifest.Cities)
        {
            var details = await _manifestClient.GetCityManifestAsync(city.ManifestPath);
            Assume.That(details, Is.Not.Null);

            Assert.That(details!.CityRemoteId, Is.EqualTo(city.RemoteId),
                $"CityRemoteId mismatch for city '{city.Name}'");
        }
    }

    [Test]
    public async Task CityDetailsManifest_HasAtLeastOneMonument_ForEachCity()
    {
        foreach (var city in _citiesManifest.Cities)
        {
            var details = await _manifestClient.GetCityManifestAsync(city.ManifestPath);
            Assume.That(details, Is.Not.Null);

            Assert.That(details!.Monuments, Is.Not.Empty,
                $"City '{city.Name}' has no monuments in its details manifest");
        }
    }

    [Test]
    public async Task CityDetailsManifest_EachMonument_HasRequiredFields()
    {
        foreach (var city in _citiesManifest.Cities)
        {
            var details = await _manifestClient.GetCityManifestAsync(city.ManifestPath);
            Assume.That(details, Is.Not.Null);

            Assert.Multiple(() =>
            {
                foreach (var monument in details!.Monuments)
                {
                    Assert.That(monument.Name, Is.Not.Null.And.Not.Empty,
                        $"A monument in city '{city.Name}' is missing Name");
                    Assert.That(monument.ImagePath, Is.Not.Null.And.Not.Empty,
                        $"Monument '{monument.Name}' in city '{city.Name}' is missing ImagePath");
                }
            });
        }
    }
}
