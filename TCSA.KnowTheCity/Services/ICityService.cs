using TCSA.KnowTheCity.Core.Models;

namespace TCSA.KnowTheCity.Services;

public interface ICityService
{
    Task<List<City>> GetCitiesAsync();
    Task<City?> GetCityByIdAsync(int id);
    Task<List<Landmark>> GetLandmarksAsync(int cityId);
}