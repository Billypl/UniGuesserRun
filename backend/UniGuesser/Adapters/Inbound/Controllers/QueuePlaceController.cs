using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniGuesser.Application.Models.PlaceModels;
using UniGuesser.Application.UseCases.Places.AddNewPlace;
using UniGuesser.Application.UseCases.Places.ChangeQueueStatus;
using UniGuesser.Application.UseCases.Places.DeletePlace;
using UniGuesser.Application.UseCases.Places.GetAllPlaces;


namespace UniGuesser.Adapters.Inbound.Controllers
{
    [Route("api/place/to_check")]
    [ApiController]
    public class PlaceToCheckController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PlaceToCheckController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddNewPlaceToQueue([FromBody] NewPlaceDto newPlace)
        {
            var command = new AddNewPlaceCommand(newPlace, true);
            var result = await _mediator.Send(command);

            return Ok(new { Message = "Place successfully added to queue" });
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ShowPlaceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPlacesInQueue()
        {
            var command = new GetAllPlacesQuery(true);
            var placesToCheck = await _mediator.Send(command);
            return Ok(placesToCheck);
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpDelete("reject/{placeId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectPlaceToCheck([FromRoute] string placeId)
        {
            var command = new DeletePlaceCommand(placeId);
            var result = await _mediator.Send(command);
            return Ok(new { Message = "Place successfully rejected" });
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost("approve/{placeId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> AcceptPlaceToCheck([FromRoute] string placeId)
        {
            var command = new ChangeQueueStatusCommand(placeId,false);
            await _mediator.Send(command);
            return Ok(new { Message = "Place successfully added to places" });
        }

    }
}
