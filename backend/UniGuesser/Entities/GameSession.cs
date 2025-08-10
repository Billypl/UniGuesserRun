using AutoMapper;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities;
using Models;
using Models.Enumerations;
using Models.GameModels;
using Models.PlaceModels;
using UniGuesser.Middleware.Exceptions;
using UniGuesser.Services;

public class GameSession
{
    [Key]
    public int Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Guid { get; set; } = Guid.NewGuid();
    public virtual List<Round> Rounds { get; set; }

    public GameMode GameMode { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int ActualRoundNumber { get; set; }
    
    public double GameScore { get; set; }
    public string Difficulty { get; set; }

    public int? UserId { get; set; }
    public virtual User? Player { get; set; }

    public GameStatus GameState { get; set; } = GameStatus.InProgress;

    public Round GetRoundOrThrow(int requestedRound)
    {
        if (ActualRoundNumber != requestedRound)
            throw new GameSessionExceptions.WrongRequestedRoundException(ActualRoundNumber, requestedRound);

        return Rounds[requestedRound];
    }

    public RoundResultDto CheckGuess(Coordinates guess, IMapper mapper, int totalRounds)
    {
        if (ActualRoundNumber >= totalRounds)
            throw new GameSessionExceptions.RoundNumberOverflowException(ActualRoundNumber);

        var round = Rounds[ActualRoundNumber];
        var place = round.PlaceToGuess;

        var distance = DistanceCalculator.CalculateDistanceBetweenCords(new Coordinates
            { Latitude = place.Latitude, Longitude = place.Longitude }, guess);

        round.Score = distance;
        round.Latitude = guess.Latitude;
        round.Longitude = guess.Longitude;

        GameScore += distance;

        var result = new RoundResultDto
        {
            DistanceDifference = distance,
            RoundNumber = ActualRoundNumber,
            OriginalPlace = mapper.Map<ShowPlaceDto>(place)
        };

        ActualRoundNumber++;
        return result;
    }

    public void EnsureGameFinished(int roundsNumber)
    {
        if (ActualRoundNumber != roundsNumber)
            throw new GameSessionExceptions.GameCannotBeFinishedException(roundsNumber - ActualRoundNumber);
    }

}