using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ScoreboardModels;
using Services;
using UniGuesser.Models.ScoreboardModels;


namespace Controllers
{
  
    [ApiController]
    [Route("api/game_sessions")]
    public class GameSessionController : ControllerBase
    {
        private readonly IGameSessionService _gameSessionService;

        public GameSessionController(IGameSessionService scoreboardService)
        {
            _gameSessionService = scoreboardService;
        }

        [HttpGet("scoreboard")]
        [ProducesResponseType(typeof(PagedResult<UserStats>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserStatsPage([FromQuery] ScoreboardQuery scoreboardQuery)
        {
            // Rezultat:
            //Items → lista elementów: ["Element11", "Element12", ..., "Element20"]
            //TotalItemsCount → Ilość wyników w bazie
            //ItemFrom → 11(pierwszy element na stronie)
            //ItemsTo → 20(ostatni element na stronie)
            //TotalPages → 3(liczba stron)

           PagedResult<UserStats> scores = 
                await _gameSessionService.GetPagedUserStatsResult(scoreboardQuery);
            return Ok(scores);
        }

        [HttpGet("history")]
        [Authorize(Roles = "Admin, Moderator, User")]
        [ProducesResponseType(typeof(PagedResult<FinishedGameDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistoryPages([FromQuery] ScoreboardQuery scoreboardQuery)
        {
            PagedResult<FinishedGameDto> scores = 
                await _gameSessionService.GetGameHistoryPage(scoreboardQuery);
            return Ok(scores);
        }

        [HttpGet("history/user/{userGuid}")]
        [Authorize(Roles = "Admin, Moderator, User")]
        [ProducesResponseType(typeof(PagedResult<FinishedGameDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistoryPagesByUser([FromQuery] UserHistoryQuery userHistoryQuery, [FromRoute] string userGuid)
        {
            PagedResult<FinishedGameDto> scores = 
                await _gameSessionService.GetGameHistoryPageByUser(userHistoryQuery, userGuid);
            return Ok(scores);
        }

        [HttpGet("{gameGuid}")]
        [ProducesResponseType(typeof(FinishedGameDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetResultDetails([FromRoute] string gameGuid)
        {
            FinishedGameDto gameResult = await _gameSessionService.GetFinishedGame(gameGuid);
            return Ok(gameResult);
        }

        [HttpDelete("{gameGuid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGame(string gameGuid)
        {
            await _gameSessionService.DeleteSessionByGuid(gameGuid);
            return Ok(new { Message = "Game successfully deleted" });
        }
    }
}
