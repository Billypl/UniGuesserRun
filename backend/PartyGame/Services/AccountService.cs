using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PartyGame.Entities;
using PartyGame.Models.AccountModels;
using PartyGame.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;

namespace PartyGame.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto registerUserDto, string Role); 
        string Login(LoginUserDto loginUserDto);

        AccountDetailsDto GetAccountDetails();

    }

    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IHttpContextAccessorService _contextAccessorService;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;

        public AccountService(IAccountRepository accountRepository, IPasswordHasher<User> passwordHasher,
            IHttpContextAccessorService contextAccessorService, IMapper mapper,
            IOptions<AuthenticationSettings> authenticationSettings)
        {
            _accountRepository =accountRepository;
            _contextAccessorService = contextAccessorService;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings.Value;

        }


        public void RegisterUser(RegisterUserDto registerUserDto,string Role)
        {
            if (_accountRepository.GetUserByNicknameOrEmailAsync(registerUserDto.Nickname).Result is not null)
            {
                throw new BadHttpRequestException("Nickname is already used");
            }

            if (_accountRepository.GetUserByNicknameOrEmailAsync(registerUserDto.Email).Result is not null)
            {
                throw new BadHttpRequestException("Email is already used");
            }

            var newUser = new User()
            {
                Email = registerUserDto.Email,
                Nickname = registerUserDto.Nickname,
                CreatedAt = DateTime.Now,
                Role = Role,

            };

            var passwordHash = _passwordHasher.HashPassword(newUser, registerUserDto.Password);
            newUser.PasswordHash = passwordHash;


            _accountRepository.CreateAsync(newUser);
        }

        public string Login(LoginUserDto loginUserDto)
        {
            var user = _accountRepository.GetUserByNicknameOrEmailAsync(loginUserDto.NicknameOrEmail).Result;

            if (user is null)
            {
                throw new KeyNotFoundException("Invalid nickname or password");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUserDto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new KeyNotFoundException("Invalid nickname or password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Email,$"{user.Email}"),
                new Claim(ClaimTypes.Role,$"{user.Role}"),
                new Claim(ClaimTypes.Name, $"{user.Nickname}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireAccount);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        public AccountDetailsDto GetAccountDetails()
        {
            AccountDetailsFromTokenDto tokenData = _contextAccessorService.GetProfileInformation();

            User account = _accountRepository.GetAsync(tokenData.UserId).Result;

            AccountDetailsDto accountDetailsDto = _mapper.Map<AccountDetailsDto>(account);

            return accountDetailsDto;
        }

    }
}
