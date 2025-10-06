using MediatR;
using Microsoft.AspNetCore.Identity;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Application.UseCases.Accounts.Delete;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Middleware.Exceptions;
using UniGuesser.Domain.Services;

namespace UniGuesser.Application.UseCases.Accounts.Login
{


    public class LoginHandler (IAccountRepository accountRepository, IPasswordHasher<User> passwordHasher,
        ITokenService accountTokenService) : IRequestHandler<LoginCommand, LoginResultDto>
    {


        public async Task<LoginResultDto> Handle(LoginCommand loginCommand, CancellationToken cancellationToken)
        {
            var user = await accountRepository.GetUserByNicknameOrEmailAsync(loginCommand.NicknameOrEmail);

            if (user is null)
            {
                throw new AccountExceptions.InvalidUsernameOrPasswordException();
            }

            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginCommand.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new AccountExceptions.InvalidUsernameOrPasswordException();
            }

            var token = accountTokenService.GenerateAccountToken(user);
            var refreshToken = accountTokenService.GenerateRefreshToken(user);

            return new LoginResultDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Nickname = user.Nickname
            };
        }

    }
}
