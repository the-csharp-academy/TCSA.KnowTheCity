namespace TCSA.KnowTheCity.Models;

public class FavoriteCity
{
    public int Id { get; set; }
    public string CityId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}