using Microsoft.EntityFrameworkCore;
using TCSA.KnowTheCity.Core.Data;
using TCSA.KnowTheCity.Core.Models.Domain;

namespace TCSA.KnowTheCity.Services;

public interface ICityService
{
    Task<List<City>> GetCitiesAsync();
    Task<City?> GetCityByIdAsync(int id);
    Task<List<Landmark>> GetLandmarksAsync(string cityRemoteId);
}

public class CityService(IDbContextFactory<KnowTheCityDbContext> dbFactory) : ICityService
{
    public async Task<List<City>> GetCitiesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Cities
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<City?> GetCityByIdAsync(int id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Cities
            .AsNoTracking()
            .Include(c => c.Landmarks)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Landmark>> GetLandmarksAsync(string cityRemoteId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var lm = await db.Landmarks.ToListAsync();

        return await db.Landmarks
            .AsNoTracking()
            .Where(l => l.City.RemoteId == cityRemoteId)
            .OrderBy(l => l.Name)
            .ToListAsync();
    }
}