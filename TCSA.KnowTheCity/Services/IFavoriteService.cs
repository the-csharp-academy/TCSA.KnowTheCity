using TCSA.KnowTheCity.Core.Models;

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