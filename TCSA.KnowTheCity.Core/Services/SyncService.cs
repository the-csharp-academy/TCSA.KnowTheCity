using Microsoft.EntityFrameworkCore;
using TCSA.KnowTheCity.Core.Clients;
using TCSA.KnowTheCity.Core.Data;
using TCSA.KnowTheCity.Core.Enums;
using TCSA.KnowTheCity.Core.Models.Domain;
using TCSA.KnowTheCity.Core.Models.DTOs;

namespace TCSA.KnowTheCity.Core.Services;

public interface ISyncService
{
    Task SyncCitiesAndMonumentsIfNeededAsync(CancellationToken cancellationToken = default);
}

public class SyncService(
    IDbContextFactory<KnowTheCityDbContext> dbFactory,
    IManifestClient manifestClient) : ISyncService
{
    private static readonly DateTime DefaultManifestDateAdded = new(2026, 3, 1);

    public async Task SyncCitiesAndMonumentsIfNeededAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);

        if (!await IsSyncRequiredAsync(db, cancellationToken))
        {
            return;
        }

        var citiesManifest = await manifestClient.GetCitiesManifestAsync(cancellationToken);

        if (citiesManifest is null || citiesManifest.Cities.Count == 0)
        {
            await UpdateLastSyncUtcAsync(db, cancellationToken);
            return;
        }

        var citiesByRemoteId = await SyncCitiesAsync(db, citiesManifest, cancellationToken);

        foreach (var cityManifest in citiesManifest.Cities)
        {
            if (!citiesByRemoteId.TryGetValue(cityManifest.RemoteId, out var city))
            {
                continue;
            }

            try
            {
                var cityManifestDetails = await manifestClient.GetCityManifestAsync(cityManifest.ManifestPath, cancellationToken);

                if (cityManifestDetails is null || cityManifestDetails.Monuments.Count == 0)
                {
                    continue;
                }

                await SyncMonumentsAsync(db, city.Id, cityManifestDetails, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching manifest for city {city.Name}: {ex.Message}");
            }
        }

        await UpdateLastSyncUtcAsync(db, cancellationToken);
    }

    private static async Task<bool> IsSyncRequiredAsync(
        KnowTheCityDbContext db,
        CancellationToken cancellationToken)
    {
        var lastConfig = await db.Configurations.FirstOrDefaultAsync(cancellationToken);

        if (lastConfig!.LastSync < DateTime.UtcNow.AddDays(-1))
        {
            return true;
        }

        return false;
    }

    private static async Task<Dictionary<string, City>> SyncCitiesAsync(
        KnowTheCityDbContext db,
        CitiesManifest manifest,
        CancellationToken cancellationToken)
    {
        var cities = await db.Cities.ToListAsync(cancellationToken);
        var citiesByRemoteId = cities.ToDictionary(x => x.RemoteId, StringComparer.OrdinalIgnoreCase);

        var hasChanges = false;

        foreach (var cityManifest in manifest.Cities)
        {
            if (citiesByRemoteId.TryGetValue(cityManifest.RemoteId, out var existingCity))
            {
                hasChanges |= ApplyCityManifest(existingCity, cityManifest);
                continue;
            }

            var newCity = CreateCity(cityManifest);
            db.Cities.Add(newCity);
            citiesByRemoteId[newCity.RemoteId] = newCity;
            hasChanges = true;
        }

        if (hasChanges)
        {
            await db.SaveChangesAsync(cancellationToken);
        }

        return citiesByRemoteId;
    }

    private static async Task SyncMonumentsAsync(
        KnowTheCityDbContext db,
        int cityId,
        CityDetailsManifest manifest,
        CancellationToken cancellationToken)
    {
        var landmarks = await db.Landmarks
            .Where(x => x.CityId == cityId)
            .ToListAsync(cancellationToken);

        var landmarksByName = landmarks.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

        var hasChanges = false;

        foreach (var monumentManifest in manifest.Monuments)
        {
            if (landmarksByName.TryGetValue(monumentManifest.Name, out var existingLandmark))
            {
                hasChanges |= ApplyMonumentManifest(existingLandmark, monumentManifest);
                continue;
            }

            db.Landmarks.Add(CreateLandmark(cityId, monumentManifest));
            hasChanges = true;
        }

        if (hasChanges)
        {
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<City>> GetNewCitiesAsync(
        KnowTheCityDbContext db,
        CitiesManifest manifest,
        CancellationToken cancellationToken)
    {
        var remoteIds = manifest.Cities
            .Select(x => x.RemoteId)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existingRemoteIds = await db.Cities
            .Where(x => remoteIds.Contains(x.RemoteId))
            .Select(x => x.RemoteId)
            .ToListAsync(cancellationToken);

        var existingSet = existingRemoteIds.ToHashSet(StringComparer.OrdinalIgnoreCase);

        return manifest.Cities
            .Where(x => !existingSet.Contains(x.RemoteId))
            .Select(CreateCity)
            .ToList();
    }

    public async Task<List<Landmark>> GetNewMonumentsAsync(
        KnowTheCityDbContext db,
        int cityId,
        CityDetailsManifest manifest,
        CancellationToken cancellationToken)
    {
        var remoteNames = manifest.Monuments
            .Select(x => x.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existingNames = await db.Landmarks
            .Where(x => x.CityId == cityId && remoteNames.Contains(x.Name))
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);

        var existingSet = existingNames.ToHashSet(StringComparer.OrdinalIgnoreCase);

        return manifest.Monuments
            .Where(x => !existingSet.Contains(x.Name))
            .Select(x => CreateLandmark(cityId, x))
            .ToList();
    }

    private static City CreateCity(CityManifestItem manifestItem)
    {
        var country = Enum.TryParse<Country>(manifestItem.Country, ignoreCase: true, out var parsedCountry)
            ? parsedCountry
            : default;

        var continent = Enum.TryParse<Continent>(manifestItem.Continent, ignoreCase: true, out var parsedContinent)
            ? parsedContinent
            : default;

        return new City
        {
            RemoteId = manifestItem.RemoteId,
            Name = manifestItem.Name,
            Country = country,
            Continent = continent,
            ImagePath = NormalizePath(manifestItem.ImagePath),
            DateAdded = manifestItem.DateAdded ?? DefaultManifestDateAdded,
            IsActive = manifestItem.IsActive
        };
    }

    private static Landmark CreateLandmark(int cityId, MonumentManifestItem manifestItem) =>
        new()
        {
            Name = manifestItem.Name,
            ImagePath = NormalizePath(manifestItem.ImagePath),
            CityId = cityId,
            DateAdded = manifestItem.DateAdded ?? DefaultManifestDateAdded
        };

    private static bool ApplyCityManifest(City city, CityManifestItem manifestItem)
    {
        var hasChanges = false;

        var country = Enum.TryParse<Country>(manifestItem.Country, ignoreCase: true, out var parsedCountry)
            ? parsedCountry
            : default;

        var continent = Enum.TryParse<Continent>(manifestItem.Continent, ignoreCase: true, out var parsedContinent)
            ? parsedContinent
            : default;

        var imagePath = NormalizePath(manifestItem.ImagePath);
        var dateAdded = manifestItem.DateAdded ?? DefaultManifestDateAdded;

        if (!string.Equals(city.Name, manifestItem.Name, StringComparison.Ordinal))
        {
            city.Name = manifestItem.Name;
            hasChanges = true;
        }

        if (city.Country != country)
        {
            city.Country = country;
            hasChanges = true;
        }

        if (city.Continent != continent)
        {
            city.Continent = continent;
            hasChanges = true;
        }

        if (!string.Equals(city.ImagePath, imagePath, StringComparison.Ordinal))
        {
            city.ImagePath = imagePath;
            hasChanges = true;
        }

        if (city.DateAdded != dateAdded)
        {
            city.DateAdded = dateAdded;
            hasChanges = true;
        }

        if (city.IsActive != manifestItem.IsActive)
        {
            city.IsActive = manifestItem.IsActive;
            hasChanges = true;
        }

        return hasChanges;
    }

    private static bool ApplyMonumentManifest(Landmark landmark, MonumentManifestItem manifestItem)
    {
        var hasChanges = false;
        var imagePath = NormalizePath(manifestItem.ImagePath);
        var dateAdded = manifestItem.DateAdded ?? DefaultManifestDateAdded;

        if (!string.Equals(landmark.Name, manifestItem.Name, StringComparison.Ordinal))
        {
            landmark.Name = manifestItem.Name;
            hasChanges = true;
        }

        if (!string.Equals(landmark.ImagePath, imagePath, StringComparison.Ordinal))
        {
            landmark.ImagePath = imagePath;
            hasChanges = true;
        }

        if (landmark.DateAdded != dateAdded)
        {
            landmark.DateAdded = dateAdded;
            hasChanges = true;
        }

        return hasChanges;
    }

    private static string NormalizePath(string? path) =>
        string.IsNullOrWhiteSpace(path)
            ? string.Empty
            : path.Trim();

    private static async Task UpdateLastSyncUtcAsync(
        KnowTheCityDbContext db,
        CancellationToken cancellationToken)
    {
        var config = await db.Configurations.FirstOrDefaultAsync(cancellationToken);
        if (config is null)
        {
            return;
        }

        config.LastSync = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }
}