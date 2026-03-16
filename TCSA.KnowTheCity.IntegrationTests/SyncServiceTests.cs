using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TCSA.KnowTheCity.Core.Clients;
using TCSA.KnowTheCity.Core.Data;
using TCSA.KnowTheCity.Core.Enums;
using TCSA.KnowTheCity.Core.Models.Domain;
using TCSA.KnowTheCity.Core.Models.DTOs;
using TCSA.KnowTheCity.Core.Options;
using TCSA.KnowTheCity.Core.Services;

namespace TCSA.KnowTheCity.IntegrationTests;

[TestFixture]
public class SyncServiceTests
{
    private const string BaseCdnUrl = "https://mock-cdn.test";
    private const string FixturesPath = "Fixtures";

    // ---------------------------------------------------------------
    // Fixture loading
    // ---------------------------------------------------------------

    private static Task<string> LoadFixtureAsync(string fileName)
    {
        var path = Path.Combine(FixturesPath, fileName);
        return File.ReadAllTextAsync(path);
    }

    private static async Task<Dictionary<string, string>> BuildRoutesAsync()
    {
        return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [$"{BaseCdnUrl}/manifests/cities.json"] = await LoadFixtureAsync("cities.json"),
            [$"{BaseCdnUrl}/manifests/paris.json"]  = await LoadFixtureAsync("paris.json"),
            [$"{BaseCdnUrl}/manifests/london.json"] = await LoadFixtureAsync("london.json"),
        };
    }

    // ---------------------------------------------------------------
    // SUT helpers
    // ---------------------------------------------------------------

    private static KnowTheCityDbContext CreateDb(DateTime? lastSync = null)
    {
        var options = new DbContextOptionsBuilder<KnowTheCityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new KnowTheCityDbContext(options);

        db.Configurations.Add(new Configurations
        {
            LastSync = lastSync ?? DateTime.UtcNow.AddDays(-2)
        });

        db.SaveChanges();
        return db;
    }

    private static SyncService CreateSut(KnowTheCityDbContext db, Dictionary<string, string> routes)
    {
        var handler = new MockCdnHandler(routes);
        var httpClient = new HttpClient(handler);
        var options = Options.Create(new ConfigOptions { CdnUrl = BaseCdnUrl });
        var manifestClient = new ManifestClient(httpClient, options);
        var factory = new SyncServiceDbContextFactory(db);

        return new SyncService(factory, manifestClient);
    }

    // ---------------------------------------------------------------
    // Database constraints
    // ---------------------------------------------------------------

    [Test]
    public async Task DuplicateCityRemoteId_CannotBeAdded()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<KnowTheCityDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new KnowTheCityDbContext(options);
        await db.Database.EnsureCreatedAsync();

        db.Cities.Add(new City { RemoteId = "paris-fr", Name = "Paris" });
        await db.SaveChangesAsync();

        db.Cities.Add(new City { RemoteId = "paris-fr", Name = "Paris Duplicate" });

        Assert.That(
            async () => await db.SaveChangesAsync(),
            Throws.TypeOf<DbUpdateException>());

        Assert.That(
            await db.Cities.CountAsync(c => c.RemoteId == "paris-fr"),
            Is.EqualTo(1));
    }

    // ---------------------------------------------------------------
    // Sync guard
    // ---------------------------------------------------------------

    [Test]
    public async Task SyncNotRequired_WhenLastSyncIsRecent_ManifestIsNeverFetched()
    {
        // Arrange - last sync was moments ago, well within the 1-day window
        var db = CreateDb(lastSync: DateTime.UtcNow);
        var sut = CreateSut(db, routes: []);

        // Act
        await sut.SyncCitiesAndMonumentsIfNeededAsync();

        // Assert - nothing was added; the manifest was never contacted
        Assert.That(await db.Cities.CountAsync(), Is.EqualTo(0));
    }

    // ---------------------------------------------------------------
    // New cities
    // ---------------------------------------------------------------

    [Test]
    public async Task NewCity_InManifest_IsPersistedToDatabase()
    {
        // Arrange - only expose paris so london is absent from the routes
        var db = CreateDb();
        var routes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [$"{BaseCdnUrl}/manifests/cities.json"] = await LoadFixtureAsync("cities.json"),
            [$"{BaseCdnUrl}/manifests/paris.json"]  = await LoadFixtureAsync("paris.json"),
            [$"{BaseCdnUrl}/manifests/london.json"] = await LoadFixtureAsync("london.json"),
        };

        var sut = CreateSut(db, routes);

        // Act
        await sut.SyncCitiesAndMonumentsIfNeededAsync();

        // Assert
        var city = await db.Cities.FirstOrDefaultAsync(c => c.RemoteId == "paris-fr");
        Assert.That(city, Is.Not.Null);
        Assert.That(city!.Name, Is.EqualTo("Paris"));
    }

    [Test]
    public async Task MultipleCities_InManifest_AllArePersistedToDatabase()
    {
        // Arrange - cities.json contains paris-fr and london-uk
        var db = CreateDb();
        var sut = CreateSut(db, await BuildRoutesAsync());

        // Act
        await sut.SyncCitiesAndMonumentsIfNeededAsync();

        // Assert
        Assert.That(await db.Cities.CountAsync(), Is.EqualTo(2));
    }

    [Test]
    public async Task ExistingCity_InManifest_IsNotDuplicated()
    {
        // Arrange - paris already exists in the database before sync
        var db = CreateDb();
        db.Cities.Add(new City { RemoteId = "paris-fr", Name = "Paris" });
        await db.SaveChangesAsync();

        var sut = CreateSut(db, await BuildRoutesAsync());

        // Act
        await sut.SyncCitiesAndMonumentsIfNeededAsync();

        // Assert - paris-fr must appear exactly once; london-uk is added
        Assert.That(await db.Cities.CountAsync(), Is.EqualTo(2));
        Assert.That(await db.Cities.CountAsync(c => c.RemoteId == "paris-fr"), Is.EqualTo(1));
    }

    // ---------------------------------------------------------------
    // New monuments
    // ---------------------------------------------------------------

    [Test]
    public async Task NewMonuments_InCityManifest_ArePersistedToDatabase()
    {
        // Arrange - paris.json contains: Eiffel Tower, Louvre Museum
        var db = CreateDb();
        var sut = CreateSut(db, await BuildRoutesAsync());

        // Act
        await sut.SyncCitiesAndMonumentsIfNeededAsync();

        // Assert
        var city = await db.Cities.FirstAsync(c => c.RemoteId == "paris-fr");
        var landmarks = await db.Landmarks.Where(l => l.CityId == city.Id).ToListAsync();

        Assert.That(landmarks, Has.Count.EqualTo(2));
        Assert.That(landmarks.Select(l => l.Name),
            Is.EquivalentTo(new[] { "Eiffel Tower", "Louvre Museum" }));
    }

    [Test]
    public async Task ExistingMonument_InCityManifest_IsNotDuplicated()
    {
        // Arrange - Eiffel Tower already seeded; Louvre Museum is new
        var db = CreateDb();
        var city = new City { RemoteId = "paris-fr", Name = "Paris" };
        db.Cities.Add(city);
        await db.SaveChangesAsync();

        db.Landmarks.Add(new Landmark { Name = "Eiffel Tower", CityId = city.Id });
        await db.SaveChangesAsync();

        var sut = CreateSut(db, await BuildRoutesAsync());

        // Act
        await sut.SyncCitiesAndMonumentsIfNeededAsync();

        // Assert - Eiffel Tower not duplicated; Louvre Museum added
        var landmarks = await db.Landmarks.Where(l => l.CityId == city.Id).ToListAsync();
        Assert.That(landmarks, Has.Count.EqualTo(2));
        Assert.That(landmarks.Count(l => l.Name == "Eiffel Tower"), Is.EqualTo(1));
        Assert.That(landmarks.Any(l => l.Name == "Louvre Museum"), Is.True);
    }

    [Test]
    public async Task NewMonuments_AcrossMultipleCities_AreAllPersisted()
    {
        // Arrange - paris.json has 2 monuments, london.json has 1 ? total 3
        var db = CreateDb();
        var sut = CreateSut(db, await BuildRoutesAsync());

        // Act
        await sut.SyncCitiesAndMonumentsIfNeededAsync();

        // Assert
        Assert.That(await db.Landmarks.CountAsync(), Is.EqualTo(3));
    }

    // ---------------------------------------------------------------
    // LastSync timestamp
    // ---------------------------------------------------------------

    [Test]
    public async Task AfterSync_LastSyncTimestamp_IsUpdated()
    {
        // Arrange
        var db = CreateDb();
        var before = DateTime.UtcNow;
        var sut = CreateSut(db, await BuildRoutesAsync());

        // Act
        await sut.SyncCitiesAndMonumentsIfNeededAsync();

        // Assert
        var config = await db.Configurations.FirstAsync();
        Assert.That(config.LastSync, Is.GreaterThanOrEqualTo(before));
    }

    // ---------------------------------------------------------------
    // GetNewCitiesAsync
    // ---------------------------------------------------------------

    [Test]
    public async Task GetNewCitiesAsync_WhenNoCitiesExist_ReturnsAllManifestCities()
    {
        // Arrange
        var db = CreateDb();
        var manifest = new CitiesManifest
        {
            Cities =
            [
                new CityManifestItem { RemoteId = "paris-fr", Name = "Paris", Country = "France" },
                new CityManifestItem { RemoteId = "london-uk", Name = "London", Country = "UnitedKingdom" },
            ]
        };

        var sut = CreateSut(db, routes: []);

        // Act
        var result = await sut.GetNewCitiesAsync(db, manifest, CancellationToken.None);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(c => c.RemoteId),
            Is.EquivalentTo(new[] { "paris-fr", "london-uk" }));
    }

    [Test]
    public async Task GetNewCitiesAsync_WhenCityAlreadyExists_ExcludesIt()
    {
        // Arrange
        var db = CreateDb();
        db.Cities.Add(new City { RemoteId = "paris-fr", Name = "Paris" });
        await db.SaveChangesAsync();

        var manifest = new CitiesManifest
        {
            Cities =
            [
                new CityManifestItem { RemoteId = "paris-fr", Name = "Paris", Country = "France" },
                new CityManifestItem { RemoteId = "london-uk", Name = "London", Country = "UnitedKingdom" },
            ]
        };

        var sut = CreateSut(db, routes: []);

        // Act
        var result = await sut.GetNewCitiesAsync(db, manifest, CancellationToken.None);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].RemoteId, Is.EqualTo("london-uk"));
    }

    [Test]
    public async Task GetNewCitiesAsync_WhenAllCitiesExist_ReturnsEmpty()
    {
        // Arrange
        var db = CreateDb();
        db.Cities.AddRange(
            new City { RemoteId = "paris-fr", Name = "Paris" },
            new City { RemoteId = "london-uk", Name = "London" });
        await db.SaveChangesAsync();

        var manifest = new CitiesManifest
        {
            Cities =
            [
                new CityManifestItem { RemoteId = "paris-fr", Name = "Paris", Country = "France" },
                new CityManifestItem { RemoteId = "london-uk", Name = "London", Country = "UnitedKingdom" },
            ]
        };

        var sut = CreateSut(db, routes: []);

        // Act
        var result = await sut.GetNewCitiesAsync(db, manifest, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetNewCitiesAsync_MapsCountryEnum_Correctly()
    {
        // Arrange
        var db = CreateDb();
        var manifest = new CitiesManifest
        {
            Cities =
            [
                new CityManifestItem { RemoteId = "paris-fr", Name = "Paris", Country = "France" },
                new CityManifestItem { RemoteId = "tokyo-jp", Name = "Tokyo", Country = "Japan" },
            ]
        };

        var sut = CreateSut(db, routes: []);

        // Act
        var result = await sut.GetNewCitiesAsync(db, manifest, CancellationToken.None);

        // Assert
        Assert.That(result.First(c => c.RemoteId == "paris-fr").Country, Is.EqualTo(Country.France));
        Assert.That(result.First(c => c.RemoteId == "tokyo-jp").Country, Is.EqualTo(Country.Japan));
    }

    [Test]
    public async Task GetNewCitiesAsync_UnknownCountry_DefaultsToEnumDefault()
    {
        // Arrange
        var db = CreateDb();
        var manifest = new CitiesManifest
        {
            Cities =
            [
                new CityManifestItem { RemoteId = "xx-xx", Name = "Unknown City", Country = "Atlantis" },
            ]
        };

        var sut = CreateSut(db, routes: []);

        // Act
        var result = await sut.GetNewCitiesAsync(db, manifest, CancellationToken.None);

        // Assert
        Assert.That(result[0].Country, Is.EqualTo(default(Country)));
    }

    // ---------------------------------------------------------------
    // GetNewMonumentsAsync
    // ---------------------------------------------------------------

    [Test]
    public async Task GetNewMonumentsAsync_WhenNoLandmarksExist_ReturnsAllManifestMonuments()
    {
        // Arrange
        var db = CreateDb();
        var manifest = new CityDetailsManifest
        {
            Monuments =
            [
                new MonumentManifestItem { Name = "Eiffel Tower" },
                new MonumentManifestItem { Name = "Louvre Museum" },
            ]
        };

        var sut = CreateSut(db, routes: []);

        // Act
        var result = await sut.GetNewMonumentsAsync(db, cityId: 1, manifest, CancellationToken.None);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(l => l.Name),
            Is.EquivalentTo(new[] { "Eiffel Tower", "Louvre Museum" }));
    }

    [Test]
    public async Task GetNewMonumentsAsync_WhenLandmarkAlreadyExists_ExcludesIt()
    {
        // Arrange
        var db = CreateDb();
        var city = new City { RemoteId = "paris-fr", Name = "Paris" };
        db.Cities.Add(city);
        await db.SaveChangesAsync();

        db.Landmarks.Add(new Landmark { Name = "Eiffel Tower", CityId = city.Id });
        await db.SaveChangesAsync();

        var manifest = new CityDetailsManifest
        {
            Monuments =
            [
                new MonumentManifestItem { Name = "Eiffel Tower" },
                new MonumentManifestItem { Name = "Louvre Museum" },
            ]
        };

        var sut = CreateSut(db, routes: []);

        // Act
        var result = await sut.GetNewMonumentsAsync(db, city.Id, manifest, CancellationToken.None);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Louvre Museum"));
    }

    [Test]
    public async Task GetNewMonumentsAsync_WhenAllLandmarksExist_ReturnsEmpty()
    {
        // Arrange
        var db = CreateDb();
        var city = new City { RemoteId = "paris-fr", Name = "Paris" };
        db.Cities.Add(city);
        await db.SaveChangesAsync();

        db.Landmarks.AddRange(
            new Landmark { Name = "Eiffel Tower", CityId = city.Id },
            new Landmark { Name = "Louvre Museum", CityId = city.Id });
        await db.SaveChangesAsync();

        var manifest = new CityDetailsManifest
        {
            Monuments =
            [
                new MonumentManifestItem { Name = "Eiffel Tower" },
                new MonumentManifestItem { Name = "Louvre Museum" },
            ]
        };

        var sut = CreateSut(db, routes: []);

        // Act
        var result = await sut.GetNewMonumentsAsync(db, city.Id, manifest, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetNewMonumentsAsync_ScopesToCorrectCity_IgnoresOtherCityLandmarks()
    {
        // Arrange - "Eiffel Tower" exists under london (cityId 2), not paris (cityId 1)
        var db = CreateDb();
        db.Cities.AddRange(
            new City { Id = 1, RemoteId = "paris-fr", Name = "Paris" },
            new City { Id = 2, RemoteId = "london-uk", Name = "London" });
        await db.SaveChangesAsync();

        db.Landmarks.Add(new Landmark { Name = "Eiffel Tower", CityId = 2 });
        await db.SaveChangesAsync();

        var manifest = new CityDetailsManifest
        {
            Monuments = [new MonumentManifestItem { Name = "Eiffel Tower" }]
        };

        var sut = CreateSut(db, routes: []);

        // Act - query is for paris (cityId 1)
        var result = await sut.GetNewMonumentsAsync(db, cityId: 1, manifest, CancellationToken.None);

        // Assert - landmark under a different city must not suppress this one
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Eiffel Tower"));
        Assert.That(result[0].CityId, Is.EqualTo(1));
    }
}