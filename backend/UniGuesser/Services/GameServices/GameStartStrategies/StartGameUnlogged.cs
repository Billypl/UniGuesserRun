using AutoMapper;
using Entities;
using Microsoft.Extensions.Options;
using Models.GameModels;
using Models.TokenModels;
using Settings;
using UniGuesser.Middleware.Exceptions;

namespace Services.GameServices.GameStartStrategies
{
    public class StartGameUnlogged : IStartGameStrategy
    {

        private readonly ITokenService _accountTokenService;
        private readonly IGameSessionService _gameSessionService;
        private readonly IGameRoundsGenerator _gameRoundsGenerator;
        private readonly AuthenticationSettings _authenticationSettings;


        public StartGameUnlogged(
            ITokenService accountService,
            IGameSessionService gameSessionService,
            IGameRoundsGenerator gameRoundsGenerator,
            IOptions<AuthenticationSettings> authenticationSettings
        )
        {
            _accountTokenService = accountService;
            _gameSessionService = gameSessionService;
            _gameRoundsGenerator = gameRoundsGenerator;
            _authenticationSettings = authenticationSettings.Value;
        }

        public async Task<StartedGameData> StartGame(StartDataDto startDataDto)
        {
            if (startDataDto.Nickname is null)
            {
                throw new GameExceptions.EmptyNicknameException();
            }

            DifficultyLevel difficulty =
                (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startDataDto.Difficulty, ignoreCase: true);

            Guid GuestGuid = Guid.NewGuid();

            GuestTokenDataDto guestTokenData = new GuestTokenDataDto
            {
                Nickname = startDataDto.Nickname,
                Difficulty = startDataDto.Difficulty,
                GameSessionId = GuestGuid.ToString(),
            };

            string newGameToken = _accountTokenService.GenerateGuestToken(guestTokenData);

            List<Round> gameRounds = await _gameRoundsGenerator.GenerateRounds(difficulty);
            GameSession gameSession = new GameSession
            {
                PublicId = GuestGuid,
                Rounds = gameRounds,
                ExpirationDate = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireGame),
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
                Token = newGameToken,
                GameGuid = GuestGuid.ToString()
            };
        }
    }
}
