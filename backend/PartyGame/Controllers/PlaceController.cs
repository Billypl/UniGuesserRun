using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PartyGame.Entities;
using PartyGame.Models.PlaceModels;
using PartyGame.Services;

namespace PartyGame.Controllers
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
        public async Task<IActionResult> GetAllPlaces(IPlaceService placeService)
        {
            List<ShowPlaceDto> places = await _placeService.GetAllPlaces();
            return Ok(places);
        }

        [HttpGet("{placeID}")]
        public async  Task<IActionResult> GetPlace([FromRoute] string placeId)
        {
            ShowPlaceDto place = await _placeService.GetPlaceById(placeId);
            return Ok(place);
        }

        [HttpDelete("{placeId}")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> DeletePlace([FromRoute] string placeId)
        {
            await _placeService.DeletePlaceById(placeId);
            return Ok(new { message = $"Place with id {placeId} deleted successfully" });
        }

        [HttpPut("{placeId}")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> UpdatePlace([FromRoute] string placeId, [FromBody] UpdatePlaceDto updateDto)
        {
             await _placeService.UpdatePlaceById(placeId, updateDto);

            return Ok(new { message = $"Place with id {placeId} updated successfully" });
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public async Task<ActionResult> AddNewPlace([FromBody] NewPlaceDto newPlace)
        {
            await _placeService.AddNewPlace(newPlace);
            return Ok(
                new
                {
                    Message = "Place successfully added to db"
                }
            );
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost("to_check")]
        public async Task<IActionResult> AddNewPlaceToQueue([FromBody] NewPlaceDto newPlace)
        {
            await _placeService.AddNewPlaceToQueue(newPlace);
            return Ok(
                new
                {
                    Message = "Place successfully added to db"
                }
            );
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet("to_check")]
        public async Task<ActionResult> GetAllPlacesInQueue()
        {
            List<PlaceToCheckDto> placesToCheck = await _placeService.GetAllPlacesInQueue();

            return Ok(placesToCheck);
        }

        [HttpDelete("to_check/reject")]
        public ActionResult RejectPlaceToCheck([FromQuery] string placeId)
        {
            _placeService.RejectPlace(placeId);
            return Ok(
                new
                {
                    Message = "Place successfully rejected"
                }
                );
        }


        [HttpPost("to_check/approve")]
        public ActionResult AcceptPlaceToCheck([FromQuery] string placeId)
        {
            _placeService.AcceptPlace(placeId);
            return Ok(
                new
                {
                    Message = "Place successfully added to places"
                }
            );
        }

    }
}
