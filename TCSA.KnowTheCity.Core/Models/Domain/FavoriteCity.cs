namespace TCSA.KnowTheCity.Core.Models.Domain;

public class FavoriteCity
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public DateTime CreatedAt { get; set; }
}