

using Microsoft.AspNetCore.Mvc;
using PartyGame.Models.GameModels;
using PartyGame.Services.GameServices;

namespace PartyGame.Controllers
{
    [Route("api/game/geolocation")]
    [ApiController]
    public class GeolocationGameController : ControllerBase
    {

        private readonly IGameGeolocationService _gameGeolocationService;

        public GeolocationGameController(IGameGeolocationService gameGeolocationService)
        {
            _gameGeolocationService = gameGeolocationService;
        }

        [HttpPost("distance")]
        public async Task<IActionResult> GetDistanceFromPlace(Coordinates coordinates)
        {
            var distance = await _gameGeolocationService.GetDistanceFromPlace(coordinates);
            return Ok(distance);
        }



    }
}
