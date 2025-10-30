using System.ComponentModel.DataAnnotations;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.Services;
using UniGuesser.Domain.ValueObjects;
using UniGuesser.Domain.ValueObjects.Enumerations;

namespace UniGuesser.Domain.Entities;

public class GameSession
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public virtual List<Round> Rounds { get; set; } = new();
    public GameMode GameMode { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int ActualRoundNumber { get; set; }
    public double GameScore { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public Guid? UserId { get; set; }
    public virtual User? Player { get; set; }
    public GameStatus GameState { get; set; } = GameStatus.InProgress;
    public DateTime FinishDateTime { get; set; }

    public Round GetRoundOrThrow(int requestedRound)
    {
        if (ActualRoundNumber != requestedRound)
            throw new GameSessionExceptions.WrongRequestedRoundException(ActualRoundNumber, requestedRound);

        return Rounds[requestedRound];
    }

    public double CheckGuess(Coordinates guess, int totalRounds)
    {
        if (ActualRoundNumber >= totalRounds)
            throw new GameSessionExceptions.RoundNumberOverflowException(ActualRoundNumber);

        var round = Rounds[ActualRoundNumber];
        var place = round.PlaceToGuess;

        var distance = DistanceCalculator.CalculateDistanceBetweenCords(
            new Coordinates { Latitude = place.Latitude, Longitude = place.Longitude }, guess);

        round.Score = distance;
        round.Latitude = guess.Latitude;
        round.Longitude = guess.Longitude;
        GameScore += distance;
        ActualRoundNumber++;

        return distance;
    }

    public void EnsureGameFinished(int roundsNumber)
    {
        if (ActualRoundNumber != roundsNumber)
            throw new GameSessionExceptions.GameCannotBeFinishedException(roundsNumber - ActualRoundNumber);
    }
}