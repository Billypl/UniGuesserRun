﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            ShowPlaceDto place = await _placeService.GetPlaceByPublicId(placeId);
            return Ok(place);
        }

        [HttpDelete("{placeId}")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> DeletePlace([FromRoute] string placeId)
        {
            await _placeService.DeletePlaceByPublicId(placeId);
            return Ok(new { Message = $"Place with id {placeId} deleted successfully" });
        }

        [HttpPut("{placeId}")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> UpdatePlace([FromRoute] string placeId, [FromBody] UpdatePlaceDto updateDto)
        {
             await _placeService.UpdatePlaceByPublicId(placeId, updateDto);
            return Ok(new { Message = $"Place with id {placeId} updated successfully" });
        }
       
        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> AddNewPlace([FromBody] NewPlaceDto newPlace)
        {
            await _placeService.AddNewPlace(newPlace);
            return Ok(new { Message = "Place successfully added to queue" });
        }

        [HttpPost("to_check")]
        public async Task<IActionResult> AddNewPlaceToQueue([FromBody] NewPlaceDto newPlace)
        {
            await _placeService.AddNewPlaceToQueue(newPlace);
            return Ok(new { Message = "Place successfully added to queue"});
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet("to_check")]
        public async Task<IActionResult> GetAllPlacesInQueue()
        {
            List<ShowPlaceDto> placesToCheck = await _placeService.GetAllPlacesInQueue();
            return Ok(placesToCheck);
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpDelete("to_check/reject/{placeId}")]
        public async Task<IActionResult> RejectPlaceToCheck([FromRoute] string placeId)
        {
            await _placeService.RejectPlace(placeId);
            return Ok(new { Message = "Place successfully rejected"} );
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost("to_check/approve/{placeId}")]
        public async Task<IActionResult> AcceptPlaceToCheck([FromRoute] string placeId)
        {
            await _placeService.AcceptPlace(placeId);
            return Ok( new { Message = "Place successfully added to places" } );
        }

    }
}
