using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.UseCases.Games.ChangeGameStatus;
using UniGuesser.Application.UseCases.Games.CheckGuess;
using UniGuesser.Application.UseCases.Games.GetActiveGame;
using UniGuesser.Application.UseCases.Games.GetActualGameState;
using UniGuesser.Application.UseCases.Games.GetPlaceToGuess;
using UniGuesser.Application.UseCases.Games.StartNewGame;
using UniGuesser.Domain.ValueObjects;
using UniGuesser.Domain.ValueObjects.Enumerations;

namespace UniGuesser.API.Controllers;

[ApiController]
[Route("api/games")]
public class GameController : ControllerBase
{
    private readonly IMediator _mediator;

    public GameController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("start")]
    [ProducesResponseType(typeof(StartedGameData), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartGame([FromBody] StartDataDto startDataDto)
    {
        var command = new StartNewGameCommand(startDataDto);
        var token = await _mediator.Send(command);
        return Ok(token);
    }

    [HttpPatch("{gameGuid}/check")]
    [Authorize(Policy = "HasGameSessionInDatabase")]
    [ProducesResponseType(typeof(RoundResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckGuess([FromRoute] string gameGuid, [FromBody] Coordinates guessingCoordinates)
    {
        var command = new CheckGuessCommand(gameGuid, guessingCoordinates);
        var result = await _mediator.Send(command);
        return Ok(result);
    }


    [HttpGet("{gameGuid}/round/{roundNumber}")]
    [Authorize(Policy = "HasGameSessionInDatabase")]
    [ProducesResponseType(typeof(GuessingPlaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetGuessingPlace([FromRoute] string gameGuid, [FromRoute] int roundNumber)
    {
        var command = new GetPlaceToGuessQuery(gameGuid, roundNumber);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // checking if game exists for a user 
    [HttpGet("active")]
    [ProducesResponseType(typeof(GameSessionStateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetActiveGameState()
    {
        var command = new GetActiveGameQuery();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // checking actual game state of a game 
    [HttpGet("{gameGuid}/game_state")]
    [Authorize(Policy = "HasGameSessionInDatabase")]
    [ProducesResponseType(typeof(GameSessionStateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGameState([FromRoute] string gameGuid)
    {
        var command = new GetActualGameStateQuery(gameGuid);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPatch("{gameGuid}/finish")]
    [Authorize(Policy = "HasGameSessionInDatabase")]
    [ProducesResponseType(typeof(FinishedGameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FinishGame([FromRoute] string gameGuid)
    {
        var command = new ChangeGameStatusCommand(gameGuid, GameStatus.Finished);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPatch("{gameGuid}/abandon")]
    [Authorize(Policy = "HasGameSessionInDatabase")]
    [ProducesResponseType(typeof(FinishedGameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AbortGame([FromRoute] string gameGuid)
    {
        var command = new ChangeGameStatusCommand(gameGuid, GameStatus.Abandoned);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}