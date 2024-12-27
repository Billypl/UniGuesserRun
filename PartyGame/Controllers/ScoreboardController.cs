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
        public ActionResult PostNewScore([FromBody] string nickname)
        {
            var result = _scoreboardService.AddNewGame(nickname).Result;

            return Ok(result);
        }

        [HttpGet]
        public ActionResult GetScores()
        {
            var scores = _scoreboardService.GetAllGames();
            return Ok(scores);
        }

    }
}
