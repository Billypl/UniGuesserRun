using Microsoft.AspNetCore.Mvc;
using PartyGame.Entities;
using PartyGame.Models;
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


        [HttpGet]
        public ActionResult GetAllPlaces(IPlaceService placeService)
        {
            var places = _placeService.GetAllPlaces().Result;
            return Ok(places);
        }

        [HttpGet("{placeID}")]
        public ActionResult GetPlace([FromRoute] int placeId)
        {
            var place = _placeService.GetPlaceById(placeId).Result;
            return Ok(place);
        }

        [HttpPost]
        public ActionResult AddNewPlace([FromBody] NewPlaceDto newPlace)
        {
            var result = _placeService.AddNewPlace(newPlace);
            return Ok();
        }

    }
}
