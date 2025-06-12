using Microsoft.AspNetCore.Authorization;

namespace PartyGame.Authorization
{
    public class HasGameSessionInDatabase:IAuthorizationRequirement
    {
        public HasGameSessionInDatabase()
        {
         
        }
    }
}
