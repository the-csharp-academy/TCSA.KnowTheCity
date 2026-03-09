namespace TCSA.KnowTheCity.Core.Models;

public class GameResult
{
    public int Id { get; set; }
    public int CityId { get; set;}
    public City City { get; set; } = default!;
    public int CorrectCount { get; set; }
    public int TotalCount { get; set; }
    public DateTime PlayedAt { get; set; }

    public string Score => $"{CorrectCount}/{TotalCount}";

    public List<GameResultItem> Items { get; set; } = [];
}