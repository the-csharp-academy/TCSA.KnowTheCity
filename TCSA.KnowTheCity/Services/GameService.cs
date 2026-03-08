using Microsoft.EntityFrameworkCore;
using TCSA.KnowTheCity.Data;
using TCSA.KnowTheCity.Models;

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
            .Include(g => g.Items)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<List<GameResult>> GetGameHistoryAsync(string? cityName = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        return await db.GameResults
            .Where(g => cityName == null || g.City == cityName)
            .Where(g => fromDate == null || g.PlayedAt >= fromDate.Value)
            .Where(g => toDate == null || g.PlayedAt <= toDate.Value.AddDays(1).AddTicks(-1))
            .OrderByDescending(g => g.PlayedAt)
            .ToListAsync();
    }
}