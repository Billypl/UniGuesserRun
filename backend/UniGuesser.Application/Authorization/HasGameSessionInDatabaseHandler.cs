using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UniGuesser.Application.Services;
using UniGuesser.Domain.Entities;

namespace UniGuesser.Application.Authorization
{
    public class HasGameSessionInDatabaseHandler
        : AuthorizationHandler<HasGameSessionInDatabase>
    {
        private readonly IGameSessionService _gameSessionService;
        private readonly IHttpContextAccessorService _httpContextAccessorService;

        public HasGameSessionInDatabaseHandler(
            IGameSessionService gameSessionService,
            IHttpContextAccessorService httpContextAccessorService)
        {
            _gameSessionService = gameSessionService;
            _httpContextAccessorService = httpContextAccessorService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            HasGameSessionInDatabase requirement)
        {
            // 1. Odczytaj HttpContext z context.Resource
            if (context.Resource is not HttpContext httpContext)
            {
                context.Fail();
                return;
            }

            // 2. Wyciągnij gameGuid z trasy
            var routeValues = httpContext.GetRouteData()?.Values;
            if (routeValues == null || 
                !routeValues.TryGetValue("gameGuid", out var guidObj) ||
                guidObj is not string gameGuidString ||
                !Guid.TryParse(gameGuidString, out var gameGuid))
            {
                if(!routeValues.TryGetValue("gameGuid", out var guidObj2)) {
                    throw(new Exception($"-1: routeValues.TryGetValue {routeValues.TryGetValue("gameGuid", out var guidObj3)}"));
                }

                if(guidObj2 is not string gameGuidString2) {
                    throw(new Exception($"2: guidObj not string {guidObj2}"));
                }

                if(!Guid.TryParse(gameGuidString2, out var gameGuid2)) { 

                    throw(new Exception($"3: gameGuidString2 {gameGuidString2}, Guid.TryParse {!Guid.TryParse(gameGuidString2, out var gameGuid4)}"));
                }

                context.Fail();
                return;
            }

            // 3. Pobierz sesję i sprawdź uprawnienia
            GameSession session;
            try
            {
                session = await _gameSessionService.GetSessionByGuid(gameGuid);
            }
            catch
            {
                throw(new Exception("333333333333333333333333333333333333333"));

                context.Fail();
                return;
            }

            var tokenType = _httpContextAccessorService.GetTokenType();
            var userGuid = _httpContextAccessorService.GetUserIdFromHeader();

            bool isAuthorized = tokenType switch
            {
                "user" => session.Player?.Id == userGuid,
                "guest" => session.Id == userGuid,
                _ => false
            };

            if (isAuthorized)
                context.Succeed(requirement);
            else
            {
                throw(new Exception("444444444444444444444444444444444444444"));
                context.Fail();
            }
        }
    }
}
