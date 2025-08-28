

using Microsoft.AspNetCore.Mvc;
using Models.GameModels;
using Services.GameServices;


namespace Controllers
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
        [ProducesResponseType(typeof(double), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDistanceFromPlace(Coordinates coordinates)
        {
            var distance = await _gameGeolocationService.GetDistanceFromPlace(coordinates);
            return Ok(distance);
        }



    }
}
