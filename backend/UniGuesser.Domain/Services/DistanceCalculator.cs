using UniGuesser.Domain.ValueObjects;

namespace UniGuesser.Domain.Services;

public static class DistanceCalculator
{
    public static double CalculateDistanceBetweenCords(Coordinates first, Coordinates second)
    {
        const double EarthRadiusMeters = 6371000.0;

        double ConvertToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        var deltaLatitude = ConvertToRadians(second.Latitude - first.Latitude);
        var deltaLongitude = ConvertToRadians(second.Longitude - first.Longitude);

        var firstLatitudeRadians = ConvertToRadians(first.Latitude);
        var secondLatitudeRadians = ConvertToRadians(second.Latitude);

        var a = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
                Math.Cos(firstLatitudeRadians) * Math.Cos(secondLatitudeRadians) *
                Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusMeters * c;
    }
}