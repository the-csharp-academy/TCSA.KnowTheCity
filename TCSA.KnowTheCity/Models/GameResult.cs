namespace TCSA.KnowTheCity.Models;

public class GameResult
{
    public int Id { get; set; }
    public string City { get; set; } = "";
    public int CorrectCount { get; set; }
    public int TotalCount { get; set; }
    public DateTime PlayedAt { get; set; }

    public string Score => $"{CorrectCount}/{TotalCount}";

    public List<GameResultItem> Items { get; set; } = [];
}