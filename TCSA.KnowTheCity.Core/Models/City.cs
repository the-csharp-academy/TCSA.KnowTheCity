using TCSA.KnowTheCity.Core.Enums;

namespace TCSA.KnowTheCity.Core.Models;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Country Country { get; set; }
    public Continent Continent { get; set; }
    public List<Landmark> Landmarks { get; set; } = [];
}
