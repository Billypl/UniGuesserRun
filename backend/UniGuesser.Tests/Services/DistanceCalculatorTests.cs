using UniGuesser.Domain.Services;
using UniGuesser.Domain.ValueObjects;

namespace UniGuesser.Tests.Services
{
    public class DistanceCalculatorTests
    {
        [Fact]
        public void CalculateDistanceBetweenCords_SameCoordinates_ReturnsZero()
        {
            // Arrange
            var coordinate1 = new Coordinates { Latitude = 52.2297, Longitude = 21.0122 };
            var coordinate2 = new Coordinates { Latitude = 52.2297, Longitude = 21.0122 };

            // Act
            var distance = DistanceCalculator.CalculateDistanceBetweenCords(coordinate1, coordinate2);

            // Assert
            Assert.Equal(0, distance, 0.01);
        }

        [Fact]
        public void CalculateDistanceBetweenCords_WarsawToBerlin_ReturnsCorrectDistance()
        {
            // Arrange - Warsaw and Berlin coordinates
            var warsaw = new Coordinates { Latitude = 52.2297, Longitude = 21.0122 };
            var berlin = new Coordinates { Latitude = 52.5200, Longitude = 13.4050 };

            // Act
            var distance = DistanceCalculator.CalculateDistanceBetweenCords(warsaw, berlin);

            // Assert - Expected distance is approximately 516 km = 516000 meters
            // Allow for wider range as actual distance is ~517-518 km
            Assert.True(distance > 517000 && distance < 519000, $"Actual distance: {distance}");
        }

        [Fact]
        public void CalculateDistanceBetweenCords_NewYorkToLondon_ReturnsCorrectDistance()
        {
            // Arrange - New York and London coordinates
            var newYork = new Coordinates { Latitude = 40.7128, Longitude = -74.0060 };
            var london = new Coordinates { Latitude = 51.5074, Longitude = -0.1278 };

            // Act
            var distance = DistanceCalculator.CalculateDistanceBetweenCords(newYork, london);

            // Assert - Expected distance is approximately 5570 km = 5570000 meters
            Assert.True(distance > 5550000 && distance < 5590000);
        }

        [Fact]
        public void CalculateDistanceBetweenCords_EquatorPoints_ReturnsCorrectDistance()
        {
            // Arrange - Two points on the equator, 1 degree apart
            var point1 = new Coordinates { Latitude = 0, Longitude = 0 };
            var point2 = new Coordinates { Latitude = 0, Longitude = 1 };

            // Act
            var distance = DistanceCalculator.CalculateDistanceBetweenCords(point1, point2);

            // Assert - 1 degree at equator is approximately 111 km = 111000 meters
            Assert.True(distance > 110000 && distance < 112000);
        }

        [Fact]
        public void CalculateDistanceBetweenCords_OppositeHemispheres_ReturnsCorrectDistance()
        {
            // Arrange - Points on opposite sides of Earth
            var north = new Coordinates { Latitude = 45.0, Longitude = 0 };
            var south = new Coordinates { Latitude = -45.0, Longitude = 0 };

            // Act
            var distance = DistanceCalculator.CalculateDistanceBetweenCords(north, south);

            // Assert - Distance should be approximately 10000 km = 10000000 meters
            Assert.True(distance > 9900000 && distance < 10100000);
        }

        [Fact]
        public void CalculateDistanceBetweenCords_SmallDistance_ReturnsCorrectDistance()
        {
            // Arrange - Very close points
            var point1 = new Coordinates { Latitude = 52.2297, Longitude = 21.0122 };
            var point2 = new Coordinates { Latitude = 52.2300, Longitude = 21.0125 };

            // Act
            var distance = DistanceCalculator.CalculateDistanceBetweenCords(point1, point2);

            // Assert - Distance should be less than 100 meters
            Assert.True(distance < 100);
        }
    }
}
