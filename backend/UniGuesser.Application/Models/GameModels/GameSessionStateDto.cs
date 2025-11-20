using UniGuesser.Domain.ValueObjects.Enumerations;

namespace UniGuesser.Application.Models.GameModels;

public class GameSessionStateDto
{
    public Guid Id { get; set; }
    public int ActualRoundNumber { get; set; }
    public double GameScore { get; set; }
    public string Difficulty { get; set; }
    public bool IsFinished { get; set; }
    public DateTime ExpirationDate { get; set; }
    public GameMode GameMode { get; set; }
    public GameStatus GameState { get; set; }
}