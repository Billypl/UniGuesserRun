using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyGame.Models;
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

        [HttpPost("saveScore")]
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
        public ActionResult GetScores()
        {
            var scores = _scoreboardService.GetAllGames();
            return Ok(scores);
        }

    }
}
