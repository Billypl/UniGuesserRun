using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.UseCases.Game.StartNewGame;
using UniGuesser.Domain.Middleware.Exceptions;
using UniGuesser.Domain.Services;

namespace UniGuesser.Domain.Services.GameServices.GameStartStrategies
{
    public interface IGameStarter
    {
        Task<StartedGameData> StartNewGame(StartNewGameCommand startDataDto);
        IStartGameStrategy ChooseStrategy(string? tokenType);
    }

    public class GameStarter(
        StartGameLogged logged,
        StartGameUnlogged loggedStrategy,
        IHttpContextAccessorService httpContextAccessorService,
        IGameSessionService gameSessionService) : IGameStarter
    {

        public async Task<StartedGameData> StartNewGame(StartNewGameCommand startDataDto)
        {
            string? tokenType = httpContextAccessorService.GetTokenTypeSafe();
            Guid? playerGuid = httpContextAccessorService.GetUserIdFromHeaderSafe();

            if (playerGuid is not null && await gameSessionService.HasActiveGameSession(playerGuid.Value))
            {
                throw new GameSessionExceptions.UserHasActiveGameSessionException(playerGuid.Value);
            }

            var strategy = ChooseStrategy(tokenType);

            return await strategy.StartGame(startDataDto);
        }

        public IStartGameStrategy ChooseStrategy(string? tokenType)
        {
            return tokenType == "user"
                ? logged
                : loggedStrategy;
        }
    }

}
