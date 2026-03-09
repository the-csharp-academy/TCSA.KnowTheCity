using Microsoft.EntityFrameworkCore;
using TCSA.KnowTheCity.Core.Models;
using TCSA.KnowTheCity.Data;

namespace TCSA.KnowTheCity.Services;

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

    public async Task<List<GameResult>> GetGameHistoryAsync(int cityId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        return await db.GameResults
            .AsNoTracking()
            .Where(g => g.City.Name == db.Cities.Where(c => c.Id == cityId).Select(c => c.Name).FirstOrDefault())
            .Where(g => fromDate == null || g.PlayedAt >= fromDate.Value)
            .Where(g => toDate == null || g.PlayedAt <= toDate.Value.AddDays(1).AddTicks(-1))
            .OrderByDescending(g => g.PlayedAt)
            .ToListAsync();
    }
}