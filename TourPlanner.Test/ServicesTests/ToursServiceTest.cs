using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Models;
using TourPlanner.Services;

namespace TourPlanner.Test.ServicesTests
{
    public class ToursServiceTest
    {
        [Fact]
        public void ResolveCoords_ReturnsCoordinates()
        {
            // Arrange
            var httpClient = new HttpClient();
            var service = new ToursService(httpClient);

            var tour = new Tour
            {
                FromLocation = "Vienna",
                ToLocation = "Graz"
            };

            // Act
            var coords = service.ResolveCoords(tour);

            // Assert
            Assert.NotNull(coords);
            Assert.Equal(2, coords.Length);
            Assert.False(string.IsNullOrWhiteSpace(coords[0]));
            Assert.False(string.IsNullOrWhiteSpace(coords[1]));
            Assert.Matches(@"^-?\d+(\.\d+)?,\s*-?\d+(\.\d+)?$", coords[0]);
            Assert.Matches(@"^-?\d+(\.\d+)?,\s*-?\d+(\.\d+)?$", coords[1]);
        }

        [Fact]
        public void CalculateDistance_ReturnsPositiveDistance()
        {
            // Arrange
            var httpClient = new HttpClient();
            var service = new ToursService(httpClient);

            var tour = new Tour
            {
                FromLocation = "Vienna",
                ToLocation = "Graz"
            };

            // Act
            var distance = service.CalculateDistance(tour);

            // Assert
            Assert.True(distance > 0);
        }

        [Fact]
        public void CalculateEstimatedTime_ReturnsPositiveTime()
        {
            // Arrange
            var httpClient = new HttpClient();
            var service = new ToursService(httpClient);

            var tour = new Tour
            {
                FromLocation = "Vienna",
                ToLocation = "Graz"
            };

            // Act
            var estimatedTime = service.CalculateEstimatedTime(tour);

            // Assert
            Assert.True(estimatedTime > 0);
        }
    }
}
