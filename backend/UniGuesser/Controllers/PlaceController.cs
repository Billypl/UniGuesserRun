using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.PlaceModels;
using Services;


namespace Controllers
{
    [Route("api/place")]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService _placeService;

        public PlaceController(IPlaceService placeService)
        {
            _placeService = placeService;
        }


        // get places wont be authorize only for developing purpose 
        [HttpGet]
        [ProducesResponseType(typeof(List<ShowPlaceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPlaces(IPlaceService placeService)
        {
            List<ShowPlaceDto> places = await _placeService.GetAllPlaces();
            return Ok(places);
        }

        [HttpGet("{placeID}")]
        [ProducesResponseType(typeof(ShowPlaceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async  Task<IActionResult> GetPlace([FromRoute] string placeId)
        {
            ShowPlaceDto place = await _placeService.GetPlaceByPublicId(placeId);
            return Ok(place);
        }

        [HttpDelete("{placeId}")]
        [Authorize(Roles = "Admin, Moderator")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePlace([FromRoute] string placeId)
        {
            await _placeService.DeletePlaceByPublicId(placeId);
            return Ok(new { Message = $"Place with id {placeId} deleted successfully" });
        }

        [HttpPut("{placeId}")]
        [Authorize(Roles = "Admin, Moderator")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePlace([FromRoute] string placeId, [FromBody] UpdatePlaceDto updateDto)
        {
             await _placeService.UpdatePlaceByPublicId(placeId, updateDto);
            return Ok(new { Message = $"Place with id {placeId} updated successfully" });
        }
       
        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddNewPlace([FromBody] NewPlaceDto newPlace)
        {
            await _placeService.AddNewPlace(newPlace);
            return Ok(new { Message = "Place successfully added to queue" });
        }

    }
}
