namespace TCSA.KnowTheCity.Models;

public class GameResultItem
{
    public int Id { get; set; }
    public int GameResultId { get; set; }
    public string Landmark { get; set; } = "";
    public bool IsCorrect { get; set; }
    public string Label { get; set; } = "";

    public GameResult GameResult { get; set; } = default!;
}