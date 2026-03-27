namespace TCSA.KnowTheCity.Core.Models.Domain;

public class LandmarkTranslation
{
    public int Id { get; set; }
    public int LandmarkId { get; set; }
    public Landmark Landmark { get; set; } = default!;
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
