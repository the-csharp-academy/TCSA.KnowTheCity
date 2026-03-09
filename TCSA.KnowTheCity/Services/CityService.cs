using Microsoft.EntityFrameworkCore;
using TCSA.KnowTheCity.Core.Models;
using TCSA.KnowTheCity.Data;

namespace TCSA.KnowTheCity.Services;

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

    public async Task<List<Landmark>> GetLandmarksAsync(int cityId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Landmarks
            .AsNoTracking()
            .Where(l => l.CityId == cityId)
            .OrderBy(l => l.Name)
            .ToListAsync();
    }
}