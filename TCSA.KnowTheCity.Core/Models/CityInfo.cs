namespace TCSA.KnowTheCity.Models;

public class CityInfo
{
    public string Id { get; set; } = string.Empty;  
    public string Name { get; set; } = string.Empty;
    public Country Country { get; set; }
    public Continent Continent { get; set; }
    public List<LandmarkInfo> Landmarks { get; set; } = [];
}
