using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.UseCases.Game.StartNewGame;
using UniGuesser.Domain.Middleware.Exceptions;
using UniGuesser.Domain.Services;
using UniGuesser.Domain.Services.GameServices.GameStartStrategies;

namespace UniGuesser.Application.UseCases.Games.StartNewGame
{
    public class StartNewGameHandler(IHttpContextAccessorService httpContextAccessorService,IGameSessionService gameSessionService, 
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
