using Microsoft.AspNetCore.Authorization;

namespace UniGuesser.Infrastructure.Authorization
{
    public class HasGameSessionInDatabase:IAuthorizationRequirement
    {
        public HasGameSessionInDatabase()
        {
         
        }
    }
}
