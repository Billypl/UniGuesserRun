using Microsoft.AspNetCore.Authorization;
using PartyGame.Repositories;

namespace PartyGame.Authorization
{
    public class HasGameSessionInDatabaseHandler :
        AuthorizationHandler<HasGameSessionInDatabase>
    {
        private readonly GameSessionRepository _gameSessionRepository;

        public HasGameSessionInDatabaseHandler(GameSessionRepository gameSessionRepository)
        {
            _gameSessionRepository = gameSessionRepository;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasGameSessionInDatabase requirement)
        {
            throw new NotImplementedException();
        }

    }
}
