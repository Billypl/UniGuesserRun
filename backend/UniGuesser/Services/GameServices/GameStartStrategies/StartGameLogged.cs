using AutoMapper;
using Entities;
using Microsoft.Extensions.Options;
using Models.AccountModels;
using Models.GameModels;
using Services;
using Services.GameServices.GameStartStrategies;
using Settings;
namespace UniGuesser.Services.GameServices.GameStartStrategies
{
    public class StartGameLogged : IStartGameStrategy
    {

        private readonly IGameSessionService _gameSessionService;
        private readonly IHttpContextAccessorService _httpContextAccessorService;
        private readonly IAccountService _accountService;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IGameRoundsGenerator _gameRoundsGenerator;


        public StartGameLogged(
            IGameSessionService gameSessionService,
            IHttpContextAccessorService httpContextAccessorService,
            IAccountService accountService,
            IOptions<AuthenticationSettings> authenticationSettings,
            IGameRoundsGenerator gameRoundsGenerator
        )
        {
            _gameSessionService = gameSessionService;
            _httpContextAccessorService = httpContextAccessorService;
            _accountService = accountService;
            _authenticationSettings = authenticationSettings.Value;
            _gameRoundsGenerator = gameRoundsGenerator;
        }


        public async Task<StartedGameData> StartGame(StartDataDto startDataDto)
        {

            DifficultyLevel difficulty =
                (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startDataDto.Difficulty, ignoreCase: true);

            List<Round> gameRounds = await _gameRoundsGenerator.GenerateRounds(difficulty);

            AccountDetailsFromTokenDto accountDetails = _httpContextAccessorService.GetAuthenticatedUserProfile();

            var user = await _accountService.GetAccountDetailsByPublicId(accountDetails.Guid);

            GameSession gameSession = new GameSession
            {
                Guid = Guid.NewGuid(),
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
                GameGuid = gameSession.Guid.ToString()
            };
        }


    }
}
