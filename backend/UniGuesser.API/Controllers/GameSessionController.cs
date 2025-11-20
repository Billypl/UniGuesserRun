using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.UseCases.GameSessions.DeleteGameSession;
using UniGuesser.Application.UseCases.GameSessions.GetGameDetails;
using UniGuesser.Application.UseCases.GameSessions.GetGameHistoryPage;
using UniGuesser.Application.UseCases.GameSessions.GetGameHistoryPageByUser;
using UniGuesser.Application.UseCases.GameSessions.GetScoreboardPage;
using UniGuesser.Domain.ValueObjects;
using UniGuesser.Infrastructure.SharedModels.ScoreboardModels;

namespace UniGuesser.API.Controllers;

[ApiController]
[Route("api/game_sessions")]
public class GameSessionController : ControllerBase
{
    private readonly IMediator _mediator;

    public GameSessionController(IMediator mediator)
    {
        _mediator = mediator;
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

        var query = new GetScoreboardPageQuery(scoreboardQuery);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("history")]
    [Authorize(Roles = "Admin, Moderator, User")]
    [ProducesResponseType(typeof(PagedResult<FinishedGameDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistoryPages([FromQuery] ScoreboardQuery scoreboardQuery)
    {
        var query = new GetGameHistoryPageQuery(scoreboardQuery);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("history/user/{userGuid}")]
    [Authorize(Roles = "Admin, Moderator, User")]
    [ProducesResponseType(typeof(PagedResult<FinishedGameDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistoryPagesByUser([FromQuery] UserHistoryQuery userHistoryQuery,
        [FromRoute] string userGuid)
    {
        var query = new GetGameHistoryPageByUserQuery(userHistoryQuery, userGuid);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("{gameGuid}")]
    [ProducesResponseType(typeof(FinishedGameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetResultDetails([FromRoute] string gameGuid)
    {
        var query = new GetGameDetailsQuery(gameGuid);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpDelete("{gameGuid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGame(string gameGuid)
    {
        var query = new DeleteGameSessionCommand(gameGuid);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}