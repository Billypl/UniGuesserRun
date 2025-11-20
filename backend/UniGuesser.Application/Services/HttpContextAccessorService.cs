using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Domain.Exceptions;

namespace UniGuesser.Application.Services;

public interface IHttpContextAccessorService
{
    string GetTokenFromHeader();
    AccountDetailsFromTokenDto GetAuthenticatedUserProfile();
    string GetTokenType();
    string? GetTokenTypeSafe();
    Guid GetUserIdFromHeader();
    Guid? GetUserIdFromHeaderSafe();
}

public class HttpContextAccessorService : IHttpContextAccessorService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextAccessorService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public string GetTokenFromHeader()
    {
        var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authorizationHeader) ||
            !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            throw new NotFoundException("Authorization header or token is missing.");

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();

        if (string.IsNullOrEmpty(token))
            throw new NotFoundException("Token was not found in the Authorization header.");

        return token;
    }

    public AccountDetailsFromTokenDto GetAuthenticatedUserProfile()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !user.Identity.IsAuthenticated)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = user.FindFirst(ClaimTypes.Email)?.Value;
        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        var nickname = user.FindFirst(ClaimTypes.Name)?.Value;

        return new AccountDetailsFromTokenDto
        {
            Guid = userIdClaim ?? throw new NotFoundException("Id not found in claims."),
            Email = email ?? throw new NotFoundException("Email not found in claims."),
            Role = role ?? throw new NotFoundException("Role not found in claims."),
            Nickname = nickname ?? throw new NotFoundException("NicknameOrEmail not found in claims.")
        };
    }

    public string GetTokenType()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var tokenType = user.FindFirst("token_type")?.Value;

        if (string.IsNullOrEmpty(tokenType)) throw new NotFoundException("Token type in token is missing.");

        return tokenType;
    }

    public string GetTokenTypeSafe()
    {
        try
        {
            return GetTokenType();
        }
        catch (NotFoundException)
        {
            return "";
        }
    }

    public Guid GetUserIdFromHeader()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim)) throw new NotFoundException("User ID not found in claims.");

        if (!Guid.TryParse(userIdClaim, out var userId)) return Guid.Empty;

        return userId;
    }

    public Guid? GetUserIdFromHeaderSafe()
    {
        try
        {
            return GetUserIdFromHeader();
        }
        catch (NotFoundException)
        {
            return null;
        }
    }
}