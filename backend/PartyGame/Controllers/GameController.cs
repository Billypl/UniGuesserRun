using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using PartyGame.Entities;
using PartyGame.Models;
using PartyGame.Services;

namespace PartyGame.Controllers
{
    [ApiController]
    [Route("api/game")]
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
          

            string token = _gameService.StartNewGame(startData);

            return Ok(new
            {
                Token = token,
                Message = "Game generated successfully"
            });
        }

        [HttpPatch("check")]
        [Authorize]
        public ActionResult CheckGuess([FromBody]  Coordinates guessingCoordinates)
        { 
           RoundResultDto result = _gameService.CheckGuess(guessingCoordinates);
           return Ok(result);
        }

        [HttpGet("round/{roundNumber}")]
        [Authorize]
        public GuessingPlaceDto GetGuessingPlace([FromRoute] int roundNumber)
        {
            return _gameService.GetPlaceToGuess(roundNumber);
        }

        [HttpGet("actual_round")]
        [Authorize]
        public int GetActualRoundNumber()
        {
            return _gameService.GetActualRoundNumber();
        }

        [HttpPatch("finish")]
        [Authorize]
        public ActionResult FinishGame()
        {
            SummarizeGameDto result = _gameService.FinishGame();
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
