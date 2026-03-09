using TCSA.KnowTheCity.Models;

namespace TCSA.KnowTheCity.Services;

public interface IFavoriteService
{
    // Cities
    Task<List<FavoriteCity>> GetFavoriteCitiesAsync();
    Task AddFavoriteCityAsync(string cityId);
    Task RemoveFavoriteCityAsync(string cityId);

    // Landmarks
    Task<List<FavoriteLandmark>> GetFavoriteLandmarksAsync(string? cityId = null);
    Task AddFavoriteLandmarkAsync(string cityId, string landmarkId);
    Task RemoveFavoriteLandmarkAsync(string cityId, string landmarkId);
}