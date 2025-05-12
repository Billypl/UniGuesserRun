using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PartyGame.Entities;
using PartyGame.Models.AccountModels;
using PartyGame.Repositories;
using Microsoft.Extensions.Options;
using PartyGame.Extensions.Exceptions;
using PartyGame.Settings;

namespace PartyGame.Services
{
    public interface IAccountService
    {
        Task RegisterUser(RegisterUserDto registerUserDto, string Role); 
        Task<LoginResultDto> Login(LoginUserDto loginUserDto);
        Task<AccountDetailsDto> GetAccountDetails();
        string RefreshSession();
        Task<User> GetAccountDetailsByPublicId(string guid);
        Task<User> GetAccountDetailsByPublicId(Guid guid);
        Task DeleteUserByGUID(string guid);
        Task DeleteUserByValueInToken();
    }

    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IHttpContextAccessorService _contextAccessorService;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly ITokenService _accountTokenService;

        public AccountService(IAccountRepository accountRepository, IPasswordHasher<User> passwordHasher,
            IHttpContextAccessorService contextAccessorService, IMapper mapper,
            IOptions<AuthenticationSettings> authenticationSettings,
            ITokenService accountTokenService)
        {
            _accountRepository =accountRepository;
            _contextAccessorService = contextAccessorService;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings.Value;
            _accountTokenService = accountTokenService;
        }
        public async Task RegisterUser(RegisterUserDto registerUserDto,string role)
        {
            if (await _accountRepository.GetUserByNicknameOrEmailAsync(registerUserDto.Nickname) is not null)
            {
                throw new BadHttpRequestException("Nickname is already used");
            }


            if (await _accountRepository.GetUserByNicknameOrEmailAsync(registerUserDto.Email) is not null)
            {
                throw new BadHttpRequestException("Email is already used");
            }

            var newUser = new User()
            {
                Email = registerUserDto.Email,
                Nickname = registerUserDto.Nickname,
                CreatedAt = DateTime.Now,
                Role = role,

            };

            var passwordHash = _passwordHasher.HashPassword(newUser, registerUserDto.Password);
            newUser.PasswordHash = passwordHash;


            await _accountRepository.CreateAsync(newUser);
        }

        public async Task<LoginResultDto> Login(LoginUserDto loginUserDto)
        {
            var user = await _accountRepository.GetUserByNicknameOrEmailAsync(loginUserDto.NicknameOrEmail);

            if (user is null)
            {
                throw new KeyNotFoundException("Invalid nickname or password");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUserDto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new KeyNotFoundException("Invalid nickname or password");
            }

            string token = _accountTokenService.GenerateAccountToken(user);
            string refreshToken = _accountTokenService.GenerateRefreshToken(user);

            return new LoginResultDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Nickname = user.Nickname
            };
        }

        public async Task<AccountDetailsDto> GetAccountDetails()
        {
            AccountDetailsFromTokenDto tokenData = _contextAccessorService.GetAuthenticatedUserProfile();

            User? account = await _accountRepository.GetByPublicIdAsync(tokenData.UserId);

            if(account is null)
            {
                throw new NotFoundException($"User was not found");
            }

            AccountDetailsDto accountDetailsDto = _mapper.Map<AccountDetailsDto>(account);

            return accountDetailsDto;
        }

        public string RefreshSession()
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetAccountDetailsByPublicId(string id)
        {
            User? user = await _accountRepository.GetByPublicIdAsync(id);

            if(user is null)
            {
                throw new NotFoundException($"User width id {id} doesnt exist");
            }

            return user;
        }
        public async Task<User> GetAccountDetailsByPublicId(Guid id)
        {
            User? user = await _accountRepository.GetByPublicIdAsync(id);

            if (user is null)
            {
                throw new NotFoundException($"User width id {id} doesnt exist");
            }

            return user;
        }

        public async Task DeleteUserByGUID(string guid)
        {
           User? user = await _accountRepository.GetByPublicIdAsync(guid);
          
           if(user is null)
           {
                throw new NotFoundException($"User with id {guid} does not exist");
           }

            await _accountRepository.DeleteAsync(user.Id);
        }

        public async Task DeleteUserByValueInToken()
        {
            var userDataFromToken = _contextAccessorService.GetAuthenticatedUserProfile();

            await DeleteUserByGUID(userDataFromToken.UserId);
        }
    }
}
