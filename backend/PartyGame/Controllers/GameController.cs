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

        [HttpPost("start")]
        public ActionResult StartGame([FromBody]  StartDataDto startData)
        {
            var resultToken = _gameService.StartNewGame(startData);

            return Ok(new
            {
                Token = resultToken,
                Message = "Game generated successfully"
            });
        }

        [HttpPatch("check")]
        [Authorize]
        public ActionResult CheckGuess([FromBody]  Coordinates guessingCoordinates)
        {
            var result = _gameService.CheckGuess(guessingCoordinates);
            
            return Ok(result);
        }

        [HttpGet("round/{roundNumber}")]
        [Authorize]
        public GuessingPlaceDto GetGuessingPlace([FromRoute] int roundNumber)
        {
            return _gameService.GetPlaceToGuess(roundNumber);
        }

        [HttpPatch("finish")]
        [Authorize]
        public ActionResult FinishGame()
        {
            var result = _gameService.FinishGame();
            return Ok(result);
        }

        [HttpDelete("delete_session")]
        [Authorize]
        public ActionResult DeleteGame()
        {
            _gameService.DeleteGame();
            return Ok(new
                {
                    Message = "Game successfully deleted"
                }
                );
        }


    }
}
