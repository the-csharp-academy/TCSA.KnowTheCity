namespace TCSA.KnowTheCity.Core.Models;

public class FavoriteLandmark
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public City City { get; set; }
    public int LandmarkId { get; set; }
    public Landmark Landmark { get; set; }
    public DateTime CreatedAt { get; set; }
}