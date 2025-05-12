using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyGame.Models.GameModels;
using PartyGame.Services;

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

        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] StartDataDto startData)
        {
            var token = await _gameService.StartNewGame(startData);
            return Ok(new { Token = token, Message = "Game generated successfully" });
        }

        [HttpPatch("check")]
        [Authorize(Policy = "HasGameInDatabase")]
        public async Task<IActionResult> CheckGuess([FromBody] Coordinates guessingCoordinates)
        {
            var result = await _gameService.CheckGuess(guessingCoordinates);
            return Ok(result);
        }

        [HttpGet("round/{roundNumber}")]
        [Authorize(Policy = "HasGameInDatabase")]
        public async Task<IActionResult> GetGuessingPlace([FromRoute] int roundNumber)
        {
            var place = await _gameService.GetPlaceToGuess(roundNumber);
            return Ok(place);
        }

        [HttpGet("game_state")]
        [Authorize(Policy = "HasGameInDatabase")]
        public async Task<IActionResult> GetGameState()
        {
            GameSessionStateDto roundNumber = await _gameSessionService.GetActualGameState();
            return Ok(roundNumber);
        }

        [HttpPatch("finish")]
        [Authorize(Policy = "HasGameInDatabase")]
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
