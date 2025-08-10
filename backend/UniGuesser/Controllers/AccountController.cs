using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models.AccountModels;
using Services;


namespace Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController:ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPut("register")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddNewUser([FromBody] RegisterUserDto registerUserDto)
        {
            await _accountService.RegisterUser(registerUserDto,"User");
            return Ok(new {Message = "User created"} );
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            LoginResultDto tokens = await _accountService.Login(loginUserDto);
            return Ok(tokens);
        }

        [Authorize(Roles = "Admin,Moderator,User")]
        [HttpGet]
        [ProducesResponseType(typeof(AccountDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserData()
        {
            AccountDetailsDto accountDetailsDto =  await _accountService.GetAccountDetails();
            return Ok(accountDetailsDto);
        }

        [HttpDelete("{userGuid}")]
        [ProducesResponseType( StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser([FromRoute] string userGuid)
        {
            await _accountService.DeleteUserByGUID(userGuid);
            return Ok();
        }


        [Authorize(Roles = "Admin,Moderator,User")]
        [HttpDelete]
        [ProducesResponseType( StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser()
        {
            await _accountService.DeleteUserByValueInToken();
            return Ok();
        }
    }
}
