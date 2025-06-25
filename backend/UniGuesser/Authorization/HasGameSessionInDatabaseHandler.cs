using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PartyGame.Entities;
using PartyGame.Services;

namespace PartyGame.Authorization
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
                context.Fail();
                return;
            }

            // 3. Pobierz sesję i sprawdź uprawnienia
            GameSession session;
            try
            {
                session = await _gameSessionService.GetSessionByGuid(gameGuid.ToString());
            }
            catch
            {
                context.Fail();
                return;
            }

            var tokenType = _httpContextAccessorService.GetTokenType();
            var userGuid = _httpContextAccessorService.GetUserIdFromHeader();

            bool isAuthorized = tokenType switch
            {
                "user" => session.Player?.PublicId.ToString() == userGuid,
                "guest" => session.PublicId.ToString() == userGuid,
                _ => false
            };

            if (isAuthorized)
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}
