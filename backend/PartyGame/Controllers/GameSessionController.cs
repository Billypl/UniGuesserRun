using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyGame.Models;
using PartyGame.Models.ScoreboardModels;
using PartyGame.Services;


namespace PartyGame.Controllers
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
        public async Task<IActionResult> GetHistoryPages([FromQuery] ScoreboardQuery scoreboardQuery)
        {
            PagedResult<FinishedGameDto> scores = 
                await _gameSessionService.GetGameHistoryPage(scoreboardQuery);
            return Ok(scores);
        }

        [HttpGet("{gameGuid}")]
        public async Task<IActionResult> GetResultDetails([FromRoute] string gameGuid)
        {
            FinishedGameDto gameResult = await _gameSessionService.GetFinishedGame(gameGuid);
            return Ok(gameResult);
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
