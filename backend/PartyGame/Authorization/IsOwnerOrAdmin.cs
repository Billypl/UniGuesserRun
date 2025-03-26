using Microsoft.AspNetCore.Authorization;

namespace PartyGame.Authorization
{
    public class IsOwnerOrAdmin: IAuthorizationRequirement
    {
    }
}
