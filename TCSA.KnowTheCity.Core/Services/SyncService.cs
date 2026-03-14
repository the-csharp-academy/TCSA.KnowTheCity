using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TCSA.KnowTheCity.Core.Clients;
using TCSA.KnowTheCity.Core.Data;
using TCSA.KnowTheCity.Core.Enums;
using TCSA.KnowTheCity.Core.Models.Domain;
using TCSA.KnowTheCity.Core.Models.DTOs;
using TCSA.KnowTheCity.Core.Options;

namespace TCSA.KnowTheCity.Core.Services;

public interface ISyncService
{
    Task SyncCitiesAndMonumentsIfNeededAsync(CancellationToken cancellationToken = default);
}

public class SyncService(
    IDbContextFactory<KnowTheCityDbContext> _dbFactory,
    IOptions<ConfigOptions> options,
    IManifestClient _manifestClient
    ) : ISyncService
{
    public async Task SyncCitiesAndMonumentsIfNeededAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

        if (!await IsSyncRequiredAsync(db, cancellationToken))
            return;

        var citiesManifest = await _manifestClient.GetCitiesManifestAsync(cancellationToken);

        if (citiesManifest is null || citiesManifest.Cities.Count == 0)
        {
            await UpdateLastSyncUtcAsync(db, cancellationToken);
            return;
        }

        var newCities = await GetNewCitiesAsync(db, citiesManifest, cancellationToken);
        if (newCities.Count > 0)
        {
            await SyncCitiesAsync(db, newCities, cancellationToken);
        }

        foreach (var cityManifest in citiesManifest.Cities)
        {
            var city = await db.Cities .FirstOrDefaultAsync(x => x.RemoteId == cityManifest.RemoteId, cancellationToken);
            if (city is null)
                continue;

            try
            {
                var cityManifestDetails = await _manifestClient.GetCityManifestAsync(cityManifest.ManifestPath, cancellationToken);

                if (cityManifestDetails is null || cityManifestDetails.Monuments.Count == 0)
                    continue;

                var newMonuments = await GetNewMonumentsAsync(
                    db,
                    city.Id,
                    cityManifestDetails,
                    cancellationToken);

                if (newMonuments.Count > 0)
                {
                    await db.Landmarks.AddRangeAsync(newMonuments, cancellationToken);
                    await db.SaveChangesAsync(cancellationToken);
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching manifest for city {city.Name}: {ex.Message}");
                continue;
            }
        }

        await UpdateLastSyncUtcAsync(db, cancellationToken);
    }

    private async Task<bool> IsSyncRequiredAsync(
       KnowTheCityDbContext db,
       CancellationToken cancellationToken)
    {
        var lastConfig = await db.Configurations.FirstOrDefaultAsync();

        if (lastConfig!.LastSync < DateTime.UtcNow.AddDays(-1)) { return true; } // TODO make this an env var

        return false;
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
            .Select(x => new City
            {
                RemoteId = x.RemoteId,
                Name = x.Name,
                Country = Enum.TryParse<Country>(x.Country, ignoreCase: true, out var country)
                    ? country
                    : default,
                IsActive = x.IsActive
            })
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
            .Select(x => new Landmark
            {
                Name = x.Name,
                CityId = cityId
            })
            .ToList();
    }

    private static async Task SyncCitiesAsync(
        KnowTheCityDbContext db,
        List<City> newCities,
        CancellationToken cancellationToken)
    {
        await db.Cities.AddRangeAsync(newCities, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task UpdateLastSyncUtcAsync(
        KnowTheCityDbContext db,
        CancellationToken cancellationToken)
    {
        var config = await db.Configurations.FirstOrDefaultAsync(cancellationToken);
        if (config is null)
            return;

        config.LastSync = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }
}