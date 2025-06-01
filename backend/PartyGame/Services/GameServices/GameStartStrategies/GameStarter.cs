using PartyGame.Models.GameModels;

namespace PartyGame.Services.GameServices.GameStartStrategies
{
    public interface IGameStarter
    {
        Task<StartedGameData> StartNewGame(StartDataDto startDataDto);
        IStartGameStrategy ChooseStrategy(string? tokenType);
    }

    public class GameStarter : IGameStarter
    {
        private readonly StartGameLogged _logged;
        private readonly StartGameUnlogged _loggedStrategy;
        private readonly IHttpContextAccessorService _httpContextAccessorService;
        private readonly IGameSessionService _gameSessionService;

        public GameStarter(
            StartGameLogged logged,
            StartGameUnlogged loggedStrategy,
            IHttpContextAccessorService httpContextAccessorService,
            IGameSessionService gameSessionService)
        {
            _logged = logged;
            _loggedStrategy = loggedStrategy;
            _httpContextAccessorService = httpContextAccessorService;
            _gameSessionService = gameSessionService;
        }

        public async Task<StartedGameData> StartNewGame(StartDataDto startDataDto)
        {
            string? tokenType = _httpContextAccessorService.GetTokenTypeSafe();
            string? playerGuid = _httpContextAccessorService.GetUserIdFromHeaderSafe();

            if (playerGuid is not null && await _gameSessionService.HasActiveGameSession(playerGuid))
                throw new InvalidOperationException($"Game for id:{playerGuid} already exists");

            var strategy = ChooseStrategy(tokenType);

            return await strategy.StartGame(startDataDto);
        }

        public IStartGameStrategy ChooseStrategy(string? tokenType)
        {
            return tokenType == "user"
                ? _logged
                : _loggedStrategy;
        }
    }

}
