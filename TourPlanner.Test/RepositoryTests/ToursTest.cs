using TourPlanner.Models;
using TourPlanner.Data;
using TourPlanner.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TourPlanner.Test.RepositoryTests
{
    public class ToursTest
    {
        [Fact]
        public async Task TourCreatedTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TourPlannerContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using var context = new TourPlannerContext(options);
            var tourRepo = new TourRepository(context);

            // Create a new tour using the default constructor
            var newTour = new Tour
            {
                Name = "TestName",
                Description = "TestDescription",
                FromLocation = "Korneuburg",
                ToLocation = "Vienna",
                TransportType = "Car",
                Distance = 16.0f,
                EstimatedTime = 200,
                RouteInformation = "testPic"
            };

            // Act
            var createdTour = await tourRepo.CreateAsync(newTour);
            var retrievedTour = await tourRepo.GetByIdAsync(createdTour.Id);

            // Assert
            Assert.NotNull(retrievedTour);
            Assert.Equal(newTour.Name, retrievedTour.Name);
            Assert.Equal(newTour.Description, retrievedTour.Description);
            Assert.Equal(newTour.FromLocation, retrievedTour.FromLocation);
            Assert.Equal(newTour.ToLocation, retrievedTour.ToLocation);
            Assert.Equal(newTour.TransportType, retrievedTour.TransportType);
            Assert.Equal(newTour.Distance, retrievedTour.Distance);
            Assert.Equal(newTour.EstimatedTime, retrievedTour.EstimatedTime);
            Assert.Equal(newTour.RouteInformation, retrievedTour.RouteInformation);
        }
    }
}