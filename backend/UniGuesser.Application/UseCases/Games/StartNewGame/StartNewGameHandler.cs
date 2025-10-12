using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Services;
using UniGuesser.Application.Services.GameStartStrategies;
using UniGuesser.Domain.Exceptions;

namespace UniGuesser.Application.UseCases.Games.StartNewGame
{
    public class StartNewGameHandler(IHttpContextAccessorService httpContextAccessorService, IGameSessionService gameSessionService,
        StartGameLogged startGameLogged, StartGameUnlogged startGameUnlogged) : IRequestHandler<StartNewGameCommand, StartedGameData>
    {
        public async Task<StartedGameData> Handle(StartNewGameCommand request, CancellationToken cancellationToken)
        {

            string? tokenType = httpContextAccessorService.GetTokenTypeSafe();
            Guid? playerGuid = httpContextAccessorService.GetUserIdFromHeaderSafe();

            if (playerGuid is not null && await gameSessionService.HasActiveGameSession(playerGuid.Value))
                throw new GameSessionExceptions.UserHasActiveGameSessionException(playerGuid.Value);

            var strategy = ChooseStrategy(tokenType);

            return await strategy.StartGame(request);
        }

        public IStartGameStrategy ChooseStrategy(string? tokenType)
        {
            return tokenType == "user"
                ? startGameLogged
                : startGameUnlogged;
        }
    }
}
