using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyGame.Entities;
using PartyGame.Models;
using PartyGame.Models.ScoreboardModels;
using PartyGame.Services;


namespace PartyGame.Controllers
{
    [Route("api/game_results")]
    [ApiController]
    public class GameResultController:ControllerBase
    {
        private readonly IScoreboardService _scoreboardService;

        public GameResultController(IScoreboardService scoreboardService)
        {
            _scoreboardService = scoreboardService;
        }

        // for testing / checking purpose 
        [HttpPost("save_score")]
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult PostNewScore([FromBody] FinishedGame finishedGame)
        {
             _scoreboardService.SaveGame(finishedGame);
            return Ok(
                new
                {
                    Message = "Score successfully added"
                });
        }

        [HttpGet("scoreboard")]
        public async Task<IActionResult> GetScoreboardPage([FromQuery] ScoreboardQuery scoreboardQuery)
        {
            // Rezultat:
            //Items → lista elementów: ["Element11", "Element12", ..., "Element20"]
            //TotalItemsCount → Ilość wyników w bazie
            //ItemFrom → 11(pierwszy element na stronie)
            //ItemsTo → 20(ostatni element na stronie)
            //TotalPages → 3(liczba stron)

            PagedResult<FinishedGameToTable> scores = await _scoreboardService.GetFinishedGamesInScoreboard(scoreboardQuery);
            return Ok(scores);
        }

        [HttpGet("history")]
        [Authorize(Roles = "Admin, Moderator, User")]
        public async Task<IActionResult> GetHistoryPages([FromQuery] ScoreboardQuery scoreboardQuery)
        {
            PagedResult<FinishedGameToTable> scores = await _scoreboardService.GetGameHistoryPage(scoreboardQuery);
            return Ok(scores);
        }

        [HttpGet("{gameResultId}")]
        public async Task<IActionResult> GetResultDetails([FromQuery] string gameResultId)
        {
            FinishedGameDto gameResult = await _scoreboardService.GetGameResult(gameResultId);

            return Ok(gameResult);
        }
    }
}
