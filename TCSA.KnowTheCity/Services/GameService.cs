using Microsoft.EntityFrameworkCore;
using TCSA.KnowTheCity.Core.Data;
using TCSA.KnowTheCity.Core.Models.Domain;

namespace TCSA.KnowTheCity.Services;

public interface IGameService
{
    Task<int> SaveGameResultAsync(GameResult gameResult);
    Task<GameResult?> GetGameResultAsync(int id);
    Task<List<GameResult>> GetGameHistoryAsync(int? cityId = null, DateTime? fromDate = null, DateTime? toDate = null);
}

public class GameService(IDbContextFactory<KnowTheCityDbContext> dbFactory) : IGameService
{
    public async Task<int> SaveGameResultAsync(GameResult gameResult)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        db.GameResults.Add(gameResult);
        await db.SaveChangesAsync();
        return gameResult.Id;
    }

    public async Task<GameResult?> GetGameResultAsync(int id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.GameResults
            .AsNoTracking()
            .Include(g => g.City)
            .Include(g => g.Items)
                .ThenInclude(i => i.Landmark)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<List<GameResult>> GetGameHistoryAsync(int? cityId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        IQueryable<GameResult> query = db.GameResults
            .Include(r => r.City)
            .AsNoTracking();

        if (cityId.HasValue)
        {
            query = query.Where(g => g.CityId == cityId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(g => g.PlayedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            var endDate = toDate.Value.AddDays(1).AddTicks(-1);
            query = query.Where(g => g.PlayedAt <= endDate);
        }

        return await query
            .OrderByDescending(g => g.PlayedAt)
            .ToListAsync();
    }
}