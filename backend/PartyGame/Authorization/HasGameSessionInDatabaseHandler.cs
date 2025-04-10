using Microsoft.AspNetCore.Authorization;
using PartyGame.Entities;
using PartyGame.Repositories;
using System.Security.Claims;

namespace PartyGame.Authorization
{
    public class HasGameSessionInDatabaseHandler :
        AuthorizationHandler<HasGameSessionInDatabase>
    {
        private readonly IGameSessionRepository _gameSessionRepository;

        public HasGameSessionInDatabaseHandler(IGameSessionRepository gameSessionRepository)
        {
            _gameSessionRepository = gameSessionRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HasGameSessionInDatabase requirement)
        {
          
            string? gameSessionID = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(gameSessionID))
            {
                context.Fail();
                return;
            }

            GameSession? gameSession = await _gameSessionRepository.GetActiveGameSession(gameSessionID);

            if (gameSession is not null)
            {
                context.Succeed(requirement);
            }
            else {
                context.Fail();
            }

        }

    }
}
