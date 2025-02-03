using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyGame.Models.AccountModels;
using PartyGame.Services;

namespace PartyGame.Controllers
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
        public IActionResult AddNewUser([FromBody] RegisterUserDto registerUserDto)
        {
            _accountService.RegisterUser(registerUserDto,"User");

            return Ok(new
                {
                    Message = "User created"
                }
            );
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUserDto loginUserDto)
        {
           string token = _accountService.Login(loginUserDto);

           return Ok(token);
        }


    }
}
