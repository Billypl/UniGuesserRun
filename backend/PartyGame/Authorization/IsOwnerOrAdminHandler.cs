using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PartyGame.Authorization
{
    public class IsOwnerOrAdminHandler : AuthorizationHandler<IsOwnerOrAdmin>
    {

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsOwnerOrAdmin requirement)
        {


            var userId = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Fail();
                return;
            }

            var resourceId = context.Resource as string; 
            if (string.IsNullOrEmpty(resourceId))
            {
                context.Fail(); 
                return;
            }
        }
    }
}

