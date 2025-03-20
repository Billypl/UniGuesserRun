using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyGame.Models.GameModels;
using PartyGame.Models.ScoreboardModels;
using PartyGame.Services;
using System.Threading.Tasks;

namespace PartyGame.Controllers
{
    [ApiController]
    [Route("api/game")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IGameSessionService _gameSessionService;

        public GameController(IGameService gameService, IGameSessionService gameSessionService)
        {
            _gameService = gameService;
            _gameSessionService = gameSessionService;
        }

        [HttpPost("start_logged")]
        [Authorize(Roles = "Admin, Moderator, User")]
        public async Task<IActionResult> StartLoggedGame([FromBody] StartDataDto startData)
        {
            var token = await _gameService.StartNewGameLogged(startData);
            return Ok(new { Token = token, Message = "Game generated successfully" });
        }

        [HttpPost("start_unlogged")]
        public async Task<IActionResult> StartUnloggedGame([FromBody] StartDataDto startData)
        {
            var token = await _gameService.StartNewGameUnlogged(startData);
            return Ok(new { Token = token, Message = "Game generated successfully" });
        }

        [HttpPatch("check")]
        [Authorize]
        public async Task<IActionResult> CheckGuess([FromBody] Coordinates guessingCoordinates)
        {
            var result = await _gameService.CheckGuess(guessingCoordinates);
            return Ok(result);
        }

        [HttpGet("round/{roundNumber}")]
        [Authorize]
        public async Task<IActionResult> GetGuessingPlace([FromRoute] int roundNumber)
        {
            var place = await _gameService.GetPlaceToGuess(roundNumber);
            return Ok(place);
        }

        [HttpGet("actual_round")]
        [Authorize]
        public async Task<IActionResult> GetActualRoundNumber()
        {
            var roundNumber = await _gameService.GetActualRoundNumber();
            return Ok(roundNumber);
        }

        [HttpPatch("finish")]
        [Authorize]
        public async Task<IActionResult> FinishGame()
        {
            var result = await _gameService.FinishGame();
            return Ok(result);
        }

        [HttpDelete("delete_session")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGame()
        {
            await _gameSessionService.DeleteSessionByHeader();
            return Ok(new { Message = "Game successfully deleted" });
        }
    }
}
