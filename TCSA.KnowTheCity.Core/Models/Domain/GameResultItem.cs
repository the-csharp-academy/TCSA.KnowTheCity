namespace TCSA.KnowTheCity.Core.Models.Domain;

public class GameResultItem
{
    public int Id { get; set; }
    public int GameResultId { get; set; }
    public int LandmarkId { get; set; }

    public Landmark Landmark { get; set; } = default!;
    public bool IsCorrect { get; set; }
    public string Label { get; set; } = string.Empty;

    public GameResult GameResult { get; set; } = default!;
}