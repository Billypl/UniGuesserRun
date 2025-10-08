using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Settings;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.UseCases.Game.StartNewGame;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Services;
namespace UniGuesser.Domain.Services.GameServices.GameStartStrategies
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


        public async Task<StartedGameData> StartGame(StartNewGameCommand startDataDto)
        {

            DifficultyLevel difficulty =
                (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startDataDto.Difficulty, ignoreCase: true);

            List<Round> gameRounds = await _gameRoundsGenerator.GenerateRounds(difficulty);

            AccountDetailsFromTokenDto accountDetails = _httpContextAccessorService.GetAuthenticatedUserProfile();

            var user = await _accountRepository.GetByPublicIdAsync(accountDetails.Guid);

            GameSession gameSession = new GameSession
            {
                PublicId = Guid.NewGuid(),
                Rounds = gameRounds,
                ExpirationDate = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireGame),
                UserId = user.Id,
                Player = user,
                Difficulty = difficulty.ToString(),
                GameMode = startDataDto.GameMode
            };

            foreach (Round gameRound in gameRounds)
            {
                gameRound.GameSession = gameSession;
            }

            await _gameSessionService.AddNewGameSession(gameSession);

            return new StartedGameData
            {
                Token = _httpContextAccessorService.GetTokenFromHeader(),
                GameGuid = gameSession.PublicId.ToString()
            };
        }


    }
}
