using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniGuesser.Application.Models.PlaceModels;
using UniGuesser.Application.UseCases.Places.AddNewPlace;
using UniGuesser.Application.UseCases.Places.DeletePlace;
using UniGuesser.Application.UseCases.Places.GetAllPlaces;
using UniGuesser.Application.UseCases.Places.GetPlace;
using UniGuesser.Application.UseCases.Places.UpdatePlace;
using UniGuesser.Domain.Services;


namespace UniGuesser.Adapters.Inbound.Controllers
{
    [Route("api/place")]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PlaceController(IMediator mediator)
        {
            _mediator = mediator;
        }


        // get places wont be authorize only for developing purpose 
        [HttpGet]
        [ProducesResponseType(typeof(List<ShowPlaceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPlaces()
        {
            var query = new GetAllPlacesQuery(false);
            var places = await _mediator.Send(query);
            return Ok(places);
        }

        [HttpGet("{placeID}")]
        [ProducesResponseType(typeof(ShowPlaceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlace([FromRoute] string placeId)
        {
            var query = new GetPlaceDetailsQuery(placeId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpDelete("{placeId}")]
        [Authorize(Roles = "Admin, Moderator")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePlace([FromRoute] string placeId)
        {
            var command = new DeletePlaceCommand(placeId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{placeId}")]
        [Authorize(Roles = "Admin, Moderator")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePlace([FromRoute] string placeId, [FromBody] UpdatePlaceDto updateDto)
        {
            var command = new UpdatePlaceCommand(updateDto,placeId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
       
        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddNewPlace([FromBody] NewPlaceDto newPlace)
        {
            var command = new AddNewPlaceCommand(newPlace,false);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

    }
}
