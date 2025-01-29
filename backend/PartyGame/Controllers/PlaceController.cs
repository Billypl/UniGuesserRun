using Microsoft.AspNetCore.Mvc;
using PartyGame.Entities;
using PartyGame.Models.GameModels;
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
            List<Place> places = _placeService.GetAllPlaces().Result;
            return Ok(places);
        }

        [HttpGet("{placeID}")]
        public ActionResult GetPlace([FromRoute] int placeId)
        {
            Place place = _placeService.GetPlaceById(placeId).Result;
            return Ok(place);
        }

        [HttpPost]
        public ActionResult AddNewPlace([FromBody] NewPlaceDto newPlace)
        {
            _placeService.AddNewPlace(newPlace);
            return Ok(
                new
                {
                    Message = "Place successfully added to db"
                }
            );

        }

    }
}
