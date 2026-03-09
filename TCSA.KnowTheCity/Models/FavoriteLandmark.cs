namespace TCSA.KnowTheCity.Models;

public class FavoriteLandmark
{
    public int Id { get; set; }
    public string CityId { get; set; } = string.Empty;
    public string LandmarkId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}