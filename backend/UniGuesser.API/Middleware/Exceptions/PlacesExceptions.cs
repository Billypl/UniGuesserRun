namespace UniGuesser.API.Middleware.Exceptions
{
    public class PlacesExceptions
    {
        public class PlaceNotFoundException(Guid placeId) :
            BaseException($"Place with id:{placeId} not found")
        {
            public override int StatusCode => 404;
        }

        public class PlaceNotFoundInQueueException(string placeId) :
            BaseException($"Place with id:{placeId} not found in queue")
        {
            public override int StatusCode => 404;
        }

        public class PlacesNotFoundException() :
            BaseException($"No places found")
        {
            public override int StatusCode => 404;
        }

        public class NotEnoughPlacesException(int roundsNumber, int placesAvailable)
            : BaseException($"Not enough places available for the game. Required: {roundsNumber}, Available: {placesAvailable}")
        {


            public override int StatusCode => 404;
        }
    }
}
