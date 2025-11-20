using MediatR;
using Microsoft.AspNetCore.Identity;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Application.Services;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.Repositories;

namespace UniGuesser.Application.UseCases.Accounts.Login;

public class LoginHandler(
    IAccountRepository accountRepository,
    IPasswordHasher<User> passwordHasher,
    ITokenService accountTokenService) : IRequestHandler<LoginCommand, LoginResultDto>
{
    public async Task<LoginResultDto> Handle(LoginCommand loginCommand, CancellationToken cancellationToken)
    {
        var user = await accountRepository.GetUserByNicknameOrEmailAsync(loginCommand.NicknameOrEmail);

        if (user is null) throw new AccountExceptions.InvalidUsernameOrPasswordException();

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginCommand.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new AccountExceptions.InvalidUsernameOrPasswordException();

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