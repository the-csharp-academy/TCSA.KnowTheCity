using Microsoft.EntityFrameworkCore;
using TCSA.KnowTheCity.Core.Models.Domain;
using TCSA.KnowTheCity.Data;

namespace TCSA.KnowTheCity.Services;

public interface IFavoriteService
{
    // Cities
    Task<List<FavoriteCity>> GetFavoriteCitiesAsync();
    Task AddFavoriteCityAsync(int cityId);
    Task RemoveFavoriteCityAsync(int cityId);

    // Landmarks
    Task<List<FavoriteLandmark>> GetFavoriteLandmarksAsync(int? cityId = null);
    Task AddFavoriteLandmarkAsync(int cityId, int landmarkId);
    Task RemoveFavoriteLandmarkAsync(int cityId, int landmarkId);
}

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

    public async Task AddFavoriteCityAsync(int cityId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        if (await db.FavoriteCities.AnyAsync(f => f.CityId == cityId))
            return;

        db.FavoriteCities.Add(new FavoriteCity { CityId = cityId, CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();
    }

    public async Task RemoveFavoriteCityAsync(int cityId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var favorite = await db.FavoriteCities.FirstOrDefaultAsync(f => f.CityId == cityId);
        if (favorite is null) return;

        db.FavoriteCities.Remove(favorite);
        await db.SaveChangesAsync();
    }

    // ?? Landmarks ?????????????????????????????????????????????????????????????

    public async Task<List<FavoriteLandmark>> GetFavoriteLandmarksAsync(int? cityId = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.FavoriteLandmarks
            .AsNoTracking()
            .Include(fl => fl.Landmark)
            .Where(f => cityId == null || f.CityId == cityId)
            .OrderBy(f => f.CityId)
            .ThenBy(f => f.LandmarkId)
            .ToListAsync();
    }

    public async Task AddFavoriteLandmarkAsync(int cityId, int landmarkId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        if (await db.FavoriteLandmarks.AnyAsync(f => f.CityId == cityId && f.LandmarkId == landmarkId))
            return;

        db.FavoriteLandmarks.Add(new FavoriteLandmark
        {
            CityId = cityId,
            LandmarkId = landmarkId,
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    public async Task RemoveFavoriteLandmarkAsync(int cityId, int landmarkId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var favorite = await db.FavoriteLandmarks
            .FirstOrDefaultAsync(f => f.CityId == cityId && f.LandmarkId == landmarkId);
        if (favorite is null) return;

        db.FavoriteLandmarks.Remove(favorite);
        await db.SaveChangesAsync();
    }
}