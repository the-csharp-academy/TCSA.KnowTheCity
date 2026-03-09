using Microsoft.EntityFrameworkCore;
using TCSA.KnowTheCity.Data;
using TCSA.KnowTheCity.Models;

namespace TCSA.KnowTheCity.Services;

public class FavoriteService(IDbContextFactory<KnowTheCityDbContext> dbFactory) : IFavoriteService
{
    // ?? Cities ????????????????????????????????????????????????????????????????

    public async Task<List<FavoriteCity>> GetFavoriteCitiesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        return await db.FavoriteCities
            .AsNoTracking()
            .OrderBy(f => f.CityId)
            .ToListAsync();
    }

    public async Task AddFavoriteCityAsync(string cityId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var alreadyExists = await db.FavoriteCities
            .AnyAsync(f => f.CityId == cityId);

        if (alreadyExists)
            return;

        db.FavoriteCities.Add(new FavoriteCity
        {
            CityId = cityId,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }

    public async Task RemoveFavoriteCityAsync(string cityId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var favorite = await db.FavoriteCities
            .FirstOrDefaultAsync(f => f.CityId == cityId);

        if (favorite is null)
            return;

        db.FavoriteCities.Remove(favorite);
        await db.SaveChangesAsync();
    }

    // ?? Landmarks ?????????????????????????????????????????????????????????????

    public async Task<List<FavoriteLandmark>> GetFavoriteLandmarksAsync(string? cityId = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        return await db.FavoriteLandmarks
            .AsNoTracking()
            .Where(f => cityId == null || f.CityId == cityId)
            .OrderBy(f => f.CityId)
            .ThenBy(f => f.LandmarkId)
            .ToListAsync();
    }

    public async Task AddFavoriteLandmarkAsync(string cityId, string landmarkId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var alreadyExists = await db.FavoriteLandmarks
            .AnyAsync(f => f.CityId == cityId && f.LandmarkId == landmarkId);

        if (alreadyExists)
            return;

        db.FavoriteLandmarks.Add(new FavoriteLandmark
        {
            CityId = cityId,
            LandmarkId = landmarkId,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }

    public async Task RemoveFavoriteLandmarkAsync(string cityId, string landmarkId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var favorite = await db.FavoriteLandmarks
            .FirstOrDefaultAsync(f => f.CityId == cityId && f.LandmarkId == landmarkId);

        if (favorite is null)
            return;

        db.FavoriteLandmarks.Remove(favorite);
        await db.SaveChangesAsync();
    }
}