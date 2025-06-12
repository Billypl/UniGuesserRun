using Microsoft.AspNetCore.Http;
using PartyGame.Entities;
using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using PartyGame.Models.AccountModels;
using PartyGame.Extensions.Exceptions;

namespace PartyGame.Services
{
    public interface IHttpContextAccessorService
    {
        string GetTokenFromHeader();
        AccountDetailsFromTokenDto GetAuthenticatedUserProfile();
        string GetTokenType();
        string? GetTokenTypeSafe();
        string GetUserIdFromHeader();
        string? GetUserIdFromHeaderSafe();
     
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

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotFoundException("Authorization header or token is missing.");
            }

            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                throw new NotFoundException("Token was not found in the Authorization header.");
            }

            return token;
        }

        public AccountDetailsFromTokenDto GetAuthenticatedUserProfile()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            var nickname = user.FindFirst(ClaimTypes.Name)?.Value;

            return new AccountDetailsFromTokenDto
            {
                UserId = userIdClaim ?? throw new NotFoundException("Id not found in claims."),
                Email = email ?? throw new NotFoundException("Email not found in claims."),
                Role = role ?? throw new NotFoundException("Role not found in claims."),
                Nickname = nickname ?? throw new NotFoundException("Nickname not found in claims.")
            };
        }

        public string GetTokenType()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var tokenType = user.FindFirst("token_type")?.Value;

            if (string.IsNullOrEmpty(tokenType))
            {
                throw new NotFoundException("Token type in token is missing.");
            }

            return tokenType.ToString();
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

        public string GetUserIdFromHeader()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new NotFoundException("User ID not found in claims.");
            }

            return userIdClaim;
        }

        public string? GetUserIdFromHeaderSafe()
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
}

