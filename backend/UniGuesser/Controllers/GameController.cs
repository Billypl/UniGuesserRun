using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Models.Enumerations;
using Models.GameModels;
using Models.ScoreboardModels;
using Services;
using Services.GameServices;

namespace Controllers
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
        [ProducesResponseType(typeof(StartedGameData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StartGame([FromBody] StartDataDto startData)
        {
            var token = await _gameService.StartNewGame(startData);
            return Ok(token);
        }

        [HttpPatch("{gameGuid}/check")]
        [Authorize(Policy = "HasGameSessionInDatabase")]
        [ProducesResponseType(typeof(RoundResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckGuess([FromRoute] string gameGuid, [FromBody] Coordinates guessingCoordinates)
        {
            var result = await _gameService.CheckGuess(gameGuid,guessingCoordinates);
            return Ok(result);
        }

        [HttpGet("{gameGuid}/round/{roundNumber}")]
        [Authorize(Policy = "HasGameSessionInDatabase")]
        [ProducesResponseType(typeof(GuessingPlaceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetGuessingPlace([FromRoute] string gameGuid, [FromRoute] int roundNumber)
        {
            var place = await _gameService.GetPlaceToGuess(gameGuid,roundNumber);
            return Ok(place);
        }

        // checking if game exists for a user 
        [HttpGet("active")]
        [ProducesResponseType(typeof(GameSessionStateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetActiveGameState()
        {
            GameSessionStateDto gameSessionStateDto = await _gameSessionService.GetActualGameStateByHeader();
            return Ok(gameSessionStateDto);
        }

        // checking actual game state of a game 
        [HttpGet("{gameGuid}/game_state")]
        [Authorize(Policy = "HasGameSessionInDatabase")]
        [ProducesResponseType(typeof(GameSessionStateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGameState([FromRoute] string gameGuid)
        {
            GameSessionStateDto gameSessionStateDto = await _gameSessionService.GetActualGameState(gameGuid);
            return Ok(gameSessionStateDto);
        }

        [HttpPatch("{gameGuid}/finish")]
        [Authorize(Policy = "HasGameSessionInDatabase")]
        [ProducesResponseType(typeof(FinishedGameDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FinishGame([FromRoute] string gameGuid)
        {
            var result = await _gameService.EndGame(gameGuid,GameStatus.Finished);
            return Ok(result);
        }

        [HttpPatch("{gameGuid}/abandon")]
        [Authorize(Policy = "HasGameSessionInDatabase")]
        [ProducesResponseType(typeof(FinishedGameDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AbortGame([FromRoute] string gameGuid)
        {
            var result = await _gameService.EndGame(gameGuid, GameStatus.Abandoned);
            return Ok(result);
        }

    }
}
