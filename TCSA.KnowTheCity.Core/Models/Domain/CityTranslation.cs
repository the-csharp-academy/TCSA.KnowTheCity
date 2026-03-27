namespace TCSA.KnowTheCity.Core.Models.Domain;

public class CityTranslation
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public City City { get; set; } = default!;
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
