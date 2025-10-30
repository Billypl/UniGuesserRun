using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniGuesser.Application.UseCases.GeolocationGames.GetDistanceFromPlace;
using UniGuesser.Domain.ValueObjects;

namespace UniGuesser.API.Controllers;

[Route("api/games/geolocation")]
[ApiController]
public class GeolocationGameController : ControllerBase
{
    private readonly IMediator _mediator;

    public GeolocationGameController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // po id gry dac ten wiesz no 
    [HttpPost("{gameId}/distance")]
    [ProducesResponseType(typeof(double), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDistanceFromPlace([FromRoute] string gameId,
        [FromBody] Coordinates playerPosition)
    {
        var query = new GetDistanceFromPlaceCommand(gameId, playerPosition);
        var distance = await _mediator.Send(query);
        return Ok(distance);
    }
}