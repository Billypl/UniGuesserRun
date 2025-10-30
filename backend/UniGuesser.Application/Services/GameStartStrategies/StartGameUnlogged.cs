using Microsoft.Extensions.Options;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Models.TokenModels;
using UniGuesser.Application.UseCases.Games.StartNewGame;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.ValueObjects.Enumerations;
using UniGuesser.Infrastructure;
using UniGuesser.Infrastructure.Settings;

namespace UniGuesser.Application.Services.GameStartStrategies;

public class StartGameUnlogged : IStartGameStrategy
{
    private readonly ITokenService _accountTokenService;
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IGameRoundsGenerator _gameRoundsGenerator;
    private readonly IGameSessionRepository _gameSessionRepository;


    public StartGameUnlogged(
        ITokenService accountService,
        IGameSessionRepository gameSessionRepository,
        IGameRoundsGenerator gameRoundsGenerator,
        IOptions<AuthenticationSettings> authenticationSettings
    )
    {
        _accountTokenService = accountService;
        _gameSessionRepository = gameSessionRepository;
        _gameRoundsGenerator = gameRoundsGenerator;
        _authenticationSettings = authenticationSettings.Value;
    }

    public async Task<StartedGameData> StartGame(StartNewGameCommand startGameCommand)
    {
        if (startGameCommand.startDataDto.Nickname is null) throw new GameExceptions.EmptyNicknameException();

        var difficulty =
            (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startGameCommand.startDataDto.Difficulty, true);

        var GuestGuid = Guid.NewGuid();

        var guestTokenData = new GuestTokenDataDto
        {
            Nickname = startGameCommand.startDataDto.Nickname,
            Difficulty = startGameCommand.startDataDto.Difficulty,
            GameSessionId = GuestGuid.ToString()
        };

        var newGameToken = _accountTokenService.GenerateGuestToken(guestTokenData);

        var gameRounds = await _gameRoundsGenerator.GenerateRounds(difficulty);
        var gameSession = new GameSession
        {
            Id = GuestGuid,
            Rounds = gameRounds,
            ExpirationDate = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireGame),
            Difficulty = difficulty,
            GameMode = startGameCommand.startDataDto.GameMode
        };

        foreach (var gameRound in gameRounds) gameRound.GameSession = gameSession;

        await _gameSessionRepository.CreateAsync(gameSession);
        return new StartedGameData
        {
            Token = newGameToken,
            GameGuid = GuestGuid.ToString()
        };
    }
}