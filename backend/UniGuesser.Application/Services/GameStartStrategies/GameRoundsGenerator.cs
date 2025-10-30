using MediatR;
using Microsoft.Extensions.Options;
using UniGuesser.Application.UseCases.Places.GetRandomPlaces;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.ValueObjects.Enumerations;
using UniGuesser.Infrastructure.Settings;

namespace UniGuesser.Application.Services.GameStartStrategies;

public interface IGameRoundsGenerator
{
    Task<List<Round>> GenerateRounds(DifficultyLevel difficulty);
}

public class GameRoundsGenerator : IGameRoundsGenerator
{
    private readonly GameSettings _gameSettings;
    private readonly IMediator _mediator;


    public GameRoundsGenerator(IMediator mediator, IOptions<GameSettings> gameSettings)
    {
        _mediator = mediator;
        _gameSettings = gameSettings.Value;
    }

    public async Task<List<Round>> GenerateRounds(DifficultyLevel difficulty)
    {
        var query = new GetRandomPlacesQuery(_gameSettings.RoundsNumber, difficulty);
        var places = await _mediator.Send(query);

        if (places.Count < _gameSettings.RoundsNumber)
            throw new PlacesExceptions.NotEnoughPlacesException(_gameSettings.RoundsNumber, places.Count);

        return places.Select(p => new Round
        {
            PlaceId = p.Id,
            PlaceToGuess = p,
            Score = 0
        }).ToList();
    }
}