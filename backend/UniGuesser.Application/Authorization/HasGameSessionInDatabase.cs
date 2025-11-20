using Microsoft.AspNetCore.Authorization;

namespace UniGuesser.Application.Authorization;

public class HasGameSessionInDatabase : IAuthorizationRequirement
{
}