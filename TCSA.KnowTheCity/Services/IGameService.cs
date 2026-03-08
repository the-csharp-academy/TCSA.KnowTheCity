using TCSA.KnowTheCity.Models;

namespace TCSA.KnowTheCity.Services;

public interface IGameService
{
    Task<int> SaveGameResultAsync(GameResult gameResult);
    Task<GameResult?> GetGameResultAsync(int id);
    Task<List<GameResult>> GetGameHistoryAsync(string? cityName = null, DateTime? fromDate = null, DateTime? toDate = null);
}