using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyGame.Models.GameModels;
using PartyGame.Services;
using PartyGame.Services.GameServices;

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
            return Ok(token);
        }

        [HttpPatch("{gameGuid}/check")]
        [Authorize(Policy = "HasGameSessionInDatabase")]
        public async Task<IActionResult> CheckGuess([FromRoute] string gameGuid, [FromBody] Coordinates guessingCoordinates)
        {
            var result = await _gameService.CheckGuess(gameGuid,guessingCoordinates);
            return Ok(result);
        }

        [HttpGet("{gameGuid}/round/{roundNumber}")]
        [Authorize(Policy = "HasGameSessionInDatabase")]
        public async Task<IActionResult> GetGuessingPlace([FromRoute] string gameGuid, [FromRoute] int roundNumber)
        {
            var place = await _gameService.GetPlaceToGuess(gameGuid,roundNumber);
            return Ok(place);
        }

        // checking if game exists for a user 
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveGameState()
        {
            GameSessionStateDto gameSessionStateDto = await _gameSessionService.GetActualGameStateByHeader();
            return Ok(gameSessionStateDto);
        }

        // checking actual game state of a game 
        [HttpGet("{gameGuid}/game_state")]
        [Authorize(Policy = "HasGameSessionInDatabase")]
        public async Task<IActionResult> GetGameState([FromRoute] string gameGuid)
        {
            GameSessionStateDto gameSessionStateDto = await _gameSessionService.GetActualGameState(gameGuid);
            return Ok(gameSessionStateDto);
        }

        [HttpPatch("{gameGuid}/finish")]
        [Authorize(Policy = "HasGameSessionInDatabase")]
        public async Task<IActionResult> FinishGame([FromRoute] string gameGuid)
        {
            var result = await _gameService.FinishGame(gameGuid);
            return Ok(result);
        }

        //[HttpPatch("{gameGuid}/abort")]
        //public async Task<IActionResult> AbortGame([FromRoute] string gameGuid)
        //{
        //    var result = await _gameService.AbortGame(gameGuid);
        //    return Ok(result);
        //}

    }
}
