using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using PartyGame.Models;
using PartyGame.Services;

namespace PartyGame.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController:ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        // TODO: add middleware 
        [HttpGet("start")]
        public StartDataDto startGame()
        {
            var result = _gameService.StartNewGame();

            return result;
        }

        [HttpPost("check")]
        [Authorize]
        public ActionResult checkGuess([FromBody]  GuessDataDto guessData)
        {
            var result = _gameService.CheckGuess(guessData);
            
            return Ok(result);
        }


    }
}
