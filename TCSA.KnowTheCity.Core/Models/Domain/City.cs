using TCSA.KnowTheCity.Core.Enums;

namespace TCSA.KnowTheCity.Core.Models.Domain;

public class City
{
    public int Id { get; set; }
    public string RemoteId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Country Country { get; set; }
    public Continent Continent { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public List<Landmark> Landmarks { get; set; } = [];
    public bool IsActive { get; set; } = true;
    public DateTime DateAdded { get; set; }
}
