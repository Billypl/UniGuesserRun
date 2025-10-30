using UniGuesser.Domain.Entities;
using UniGuesser.Domain.ValueObjects.Enumerations;

namespace UniGuesser.Application.Models.GameModels;

public class FinishedGameDto
{
    public string Id { get; set; }
    public string? UserId { get; set; }
    public string? Nickname { get; set; }
    public double FinalScore { get; set; }
    public List<Round> Rounds { get; set; }
    public string Difficulty { get; set; }
    public DateTime FinishDateTime { get; set; }
    public GameStatus GameState { get; set; }
    public GameMode GameMode { get; set; }
}