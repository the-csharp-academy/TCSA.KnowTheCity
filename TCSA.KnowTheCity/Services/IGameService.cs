using TCSA.KnowTheCity.Core.Models;

namespace TCSA.KnowTheCity.Services;

public interface IGameService
{
    Task<int> SaveGameResultAsync(GameResult gameResult);
    Task<GameResult?> GetGameResultAsync(int id);
    Task<List<GameResult>> GetGameHistoryAsync(int cityId, DateTime? fromDate = null, DateTime? toDate = null);
}