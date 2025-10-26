using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Services;
using UniGuesser.Application.Services.GameStartStrategies;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.ValueObjects.Enumerations;
using UniGuesser.Infrastructure;

namespace UniGuesser.Application.UseCases.Games.StartNewGame
{
    public class StartNewGameHandler(IHttpContextAccessorService httpContextAccessorService, IGameSessionRepository gameSessionRepository,
        StartGameLogged startGameLogged, StartGameUnlogged startGameUnlogged) : IRequestHandler<StartNewGameCommand, StartedGameData>
    {
        public async Task<StartedGameData> Handle(StartNewGameCommand request, CancellationToken cancellationToken)
        {

            string? tokenType = httpContextAccessorService.GetTokenTypeSafe();
            Guid? playerGuid = httpContextAccessorService.GetUserIdFromHeaderSafe();

            var result = await gameSessionRepository.GetActiveGameSession(playerGuid.Value);

            if (result is not null && (result.GameState == GameStatus.InProgress))
            {
                throw new GameSessionExceptions.UserHasActiveGameSessionException(playerGuid.Value);
            }

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
