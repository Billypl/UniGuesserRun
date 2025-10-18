using Microsoft.Extensions.Options;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.UseCases.Games.StartNewGame;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Repositories;
using UniGuesser.Domain.ValueObjects.Enumerations;
using UniGuesser.Infrastructure.Settings;

namespace UniGuesser.Application.Services.GameStartStrategies
{
    public class StartGameLogged : IStartGameStrategy
    {

        private readonly IGameSessionService _gameSessionService;
        private readonly IHttpContextAccessorService _httpContextAccessorService;
        private readonly IAccountRepository _accountRepository;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IGameRoundsGenerator _gameRoundsGenerator;


        public StartGameLogged(
            IGameSessionService gameSessionService,
            IHttpContextAccessorService httpContextAccessorService,
            IAccountRepository accountRepository,
            IOptions<AuthenticationSettings> authenticationSettings,
            IGameRoundsGenerator gameRoundsGenerator
        )
        {
            _gameSessionService = gameSessionService;
            _httpContextAccessorService = httpContextAccessorService;
            _accountRepository = accountRepository;
            _authenticationSettings = authenticationSettings.Value;
            _gameRoundsGenerator = gameRoundsGenerator;
        }


        public async Task<StartedGameData> StartGame(StartNewGameCommand startGameCommand)
        {

            DifficultyLevel difficulty =
                (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startGameCommand.startDataDto.Difficulty, ignoreCase: true);

            List<Round> gameRounds = await _gameRoundsGenerator.GenerateRounds(difficulty);

            AccountDetailsFromTokenDto accountDetails = _httpContextAccessorService.GetAuthenticatedUserProfile();


            if (!Guid.TryParse(accountDetails.Guid, out Guid parsedUserGuid))
            {
                throw new InvalidOperationException("Invalid user GUID found in the authenticated token.");
            }

            var user = await _accountRepository.GetAsync(parsedUserGuid);
            if (user == null)
            {
                throw new InvalidOperationException($"User not found for GUID: {parsedUserGuid}");
            }

            GameSession gameSession = new GameSession
            {
                Id = Guid.NewGuid(),
                Rounds = gameRounds,
                ExpirationDate = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireGame),
                UserId = user.Id,
                Player = user,
                Difficulty = difficulty,
                GameMode = startGameCommand.startDataDto.GameMode
            };

            foreach (Round gameRound in gameRounds)
            {
                gameRound.GameSession = gameSession;
            }

            await _gameSessionService.AddNewGameSession(gameSession);

            return new StartedGameData
            {
                Token = _httpContextAccessorService.GetTokenFromHeader(),
                GameGuid = gameSession.Id.ToString()
            };
        }


    }
}
