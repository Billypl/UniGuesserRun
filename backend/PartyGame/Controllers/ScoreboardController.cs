using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyGame.Entities;
using PartyGame.Models.ScoreboardModels;
using PartyGame.Services;


namespace PartyGame.Controllers
{
    [Route("api/scoreboard")]
    [ApiController]
    public class ScoreboardController:ControllerBase
    {
        private readonly IScoreboardService _scoreboardService;

        public ScoreboardController(IScoreboardService scoreboardService)
        {
            _scoreboardService = scoreboardService;
        }

        [HttpPost("save_score")]
        [Authorize]
        public ActionResult PostNewScore()
        {
             _scoreboardService.AddNewGame();
            return Ok(
                new
                {
                    Message = "Score successfully added"
                });
        }



        [HttpGet]
        public ActionResult GetScores([FromQuery] ScoreboardQuery scoreboardQuery)
        {
            // Rezultat:
            //Items → lista elementów: ["Element11", "Element12", ..., "Element20"]
            //TotalItemsCount → Ilość wyników w bazie
            //ItemFrom → 11(pierwszy element na stronie)
            //ItemsTo → 20(ostatni element na stronie)
            //TotalPages → 3(liczba stron)

            PagedResult<FinishedGame> scores = _scoreboardService.GetGames(scoreboardQuery).Result;
            return Ok(scores);
        }

    }
}
