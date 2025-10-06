using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UniGuesser.Domain.Services;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Application.UseCases.Accounts;
using UniGuesser.Application.UseCases.Accounts.AccountDetails;
using UniGuesser.Application.UseCases.Accounts.Delete;
using UniGuesser.Application.UseCases.Accounts.Login;
using UniGuesser.Application.UseCases.Accounts.Register;


namespace UniGuesser.Adapters.Inbound.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController:ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("register")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddNewUser([FromBody] RegisterUserDto registerUserDto)
        {
            var command = new RegisterAccountCommand(registerUserDto);
            
            await _mediator.Send(command);
            return Ok(new {Message = "Account created"} );
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            var command = new LoginCommand(loginUserDto.NicknameOrEmail, loginUserDto.Password);
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [Authorize(Roles = "Admin,Moderator,User")]
        [HttpGet("{userGuid}")]
        [ProducesResponseType(typeof(AccountDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserData([FromRoute] string userGuid)
        {
            var command = new AccountDetailsQuery(userGuid);
            var accountDetailsDto = await _mediator.Send(command);
            return Ok(accountDetailsDto);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpDelete("{userGuid}")]
        [ProducesResponseType( StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser([FromRoute] string userGuid)
        {
            var command = new DeleteCommand(userGuid);
            await _mediator.Send(command);
            return Ok(new { Message = "Account deleted" });
        }

    }
}
