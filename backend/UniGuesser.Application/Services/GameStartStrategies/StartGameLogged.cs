using Microsoft.Extensions.Options;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.UseCases.Games.StartNewGame;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Repositories;
using UniGuesser.Domain.ValueObjects.Enumerations;
using UniGuesser.Infrastructure;
using UniGuesser.Infrastructure.Settings;

namespace UniGuesser.Application.Services.GameStartStrategies;

public class StartGameLogged : IStartGameStrategy
{
    private readonly IAccountRepository _accountRepository;
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IGameRoundsGenerator _gameRoundsGenerator;

    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IHttpContextAccessorService _httpContextAccessorService;


    public StartGameLogged(
        IGameSessionRepository gameSessionRepository,
        IHttpContextAccessorService httpContextAccessorService,
        IAccountRepository accountRepository,
        IOptions<AuthenticationSettings> authenticationSettings,
        IGameRoundsGenerator gameRoundsGenerator
    )
    {
        _gameSessionRepository = gameSessionRepository;
        _httpContextAccessorService = httpContextAccessorService;
        _accountRepository = accountRepository;
        _authenticationSettings = authenticationSettings.Value;
        _gameRoundsGenerator = gameRoundsGenerator;
    }


    public async Task<StartedGameData> StartGame(StartNewGameCommand startGameCommand)
    {
        var difficulty =
            (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startGameCommand.startDataDto.Difficulty, true);

        var gameRounds = await _gameRoundsGenerator.GenerateRounds(difficulty);

        var accountDetails = _httpContextAccessorService.GetAuthenticatedUserProfile();


        if (!Guid.TryParse(accountDetails.Guid, out var parsedUserGuid))
            throw new InvalidOperationException("Invalid user GUID found in the authenticated token.");

        var user = await _accountRepository.GetAsync(parsedUserGuid);
        if (user == null) throw new InvalidOperationException($"User not found for GUID: {parsedUserGuid}");

        var gameSession = new GameSession
        {
            Id = Guid.NewGuid(),
            Rounds = gameRounds,
            ExpirationDate = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireGame),
            UserId = user.Id,
            Player = user,
            Difficulty = difficulty,
            GameMode = startGameCommand.startDataDto.GameMode
        };

        foreach (var gameRound in gameRounds) gameRound.GameSession = gameSession;

        await _gameSessionRepository.CreateAsync(gameSession);

        return new StartedGameData
        {
            Token = _httpContextAccessorService.GetTokenFromHeader(),
            GameGuid = gameSession.Id.ToString()
        };
    }
}