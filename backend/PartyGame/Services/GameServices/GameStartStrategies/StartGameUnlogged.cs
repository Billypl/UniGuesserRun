using AutoMapper;
using Microsoft.Extensions.Options;
using PartyGame.Entities;
using PartyGame.Models.AccountModels;
using PartyGame.Models.GameModels;
using PartyGame.Settings;

namespace PartyGame.Services.GameServices.GameStartStrategies
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


        public async Task<string> StartGame(StartDataDto startDataDto)
        {

            DifficultyLevel difficulty =
                (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startDataDto.Difficulty, ignoreCase: true);

            List<Round> gameRounds = await _gameRoundsGenerator.GenerateRounds(difficulty);

            AccountDetailsFromTokenDto accountDetails = _httpContextAccessorService.GetAuthenticatedUserProfile();

            var user = await _accountService.GetAccountDetailsByPublicId(accountDetails.UserId);

            GameSession gameSession = new GameSession
            {
                PublicId = Guid.Parse(accountDetails.UserId),
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
            return _httpContextAccessorService.GetTokenFromHeader();
        }


    }
}
