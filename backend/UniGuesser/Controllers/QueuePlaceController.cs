using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyGame.Models.PlaceModels;
using PartyGame.Services;

namespace PartyGame.Controllers
{
    [Route("api/place/to_check")]
    [ApiController]
    public class PlaceToCheckController : ControllerBase
    {
        private readonly IPlaceQueueService _placeQueueService;

        public PlaceToCheckController(IPlaceQueueService placeQueueService)
        {
            _placeQueueService = placeQueueService;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewPlaceToQueue([FromBody] NewPlaceDto newPlace)
        {
            await _placeQueueService.AddNewPlaceToQueue(newPlace);
            return Ok(new { Message = "Place successfully added to queue" });
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetAllPlacesInQueue()
        {
            List<ShowPlaceDto> placesToCheck = await _placeQueueService.GetAllPlacesInQueue();
            return Ok(placesToCheck);
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpDelete("reject/{placeId}")]
        public async Task<IActionResult> RejectPlaceToCheck([FromRoute] string placeId)
        {
            await _placeQueueService.RejectPlace(placeId);
            return Ok(new { Message = "Place successfully rejected" });
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost("approve/{placeId}")]
        public async Task<IActionResult> AcceptPlaceToCheck([FromRoute] string placeId)
        {
            await _placeQueueService.AcceptPlace(placeId);
            return Ok(new { Message = "Place successfully added to places" });
        }

    }
}
