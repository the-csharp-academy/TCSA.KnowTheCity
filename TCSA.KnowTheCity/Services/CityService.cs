using Microsoft.EntityFrameworkCore;
using TCSA.KnowTheCity.Core.Data;
using TCSA.KnowTheCity.Core.Enums;
using TCSA.KnowTheCity.Core.Models.Domain;

namespace TCSA.KnowTheCity.Services;

public interface ICityService
{
    Task<List<City>> GetCitiesAsync(string? city = null, Country? country = null, Continent? continent = null);
    Task<List<Country>> GetCountriesWithCitiesAsync();
    Task<City?> GetCityByIdAsync(int id);
    Task<List<Landmark>> GetLandmarksAsync(string cityRemoteId);
}

public class CityService(IDbContextFactory<KnowTheCityDbContext> dbFactory) : ICityService
{
    public async Task<List<City>> GetCitiesAsync(string? city = null, Country? country = null, Continent? continent = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        IQueryable<City> query = db.Cities.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(city))
        {
            var cityFilter = city.Trim();
            query = query.Where(c => EF.Functions.Like(c.Name, $"%{cityFilter}%"));
        }

        if (country.HasValue)
        {
            query = query.Where(c => c.Country == country.Value);
        }

        if (continent.HasValue)
        {
            query = query.Where(c => c.Continent == continent.Value);
        }

        return await query
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<Country>> GetCountriesWithCitiesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var countries = await db.Cities
            .AsNoTracking()
            .Select(c => c.Country)
            .Distinct()
            .ToListAsync();

        return countries
            .OrderBy(c => c.ToString())
            .ToList();
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