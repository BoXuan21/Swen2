using TourPlanner.Models;
using TourPlanner.Data;
using TourPlanner.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace TourPlanner.Test.RepositoryTests
{
    public class ToursTest
    {
        [Fact]
        public async Task TourCreatedAndStoredTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TourPlannerContext>()
                .UseNpgsql("Host=localhost;Database=swen2;Username=postgres;Password=postgres;Include Error Detail=true;")
                .Options;

            using var context = new TourPlannerContext(options);
            var tourRepo = new TourRepository(context);

            // Create a new tour
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

            // Verify using direct database connection
            using (var connection = new NpgsqlConnection("Host=localhost;Database=swen2;Username=postgres;Password=postgres;Include Error Detail=true;"))
            {
                await connection.OpenAsync();
                
                // Check if tour exists in database
                using var command = new NpgsqlCommand(
                    @"SELECT * FROM ""Tours"" WHERE ""Id"" = @id", connection);
                command.Parameters.AddWithValue("@id", createdTour.Id);
                
                using var reader = await command.ExecuteReaderAsync();
                
                // Assert
                Assert.True(await reader.ReadAsync(), "Tour should exist in database");
                Assert.Equal(newTour.Name, reader["Name"]);
                Assert.Equal(newTour.Description, reader["Description"]);
                Assert.Equal(newTour.FromLocation, reader["FromLocation"]);
                Assert.Equal(newTour.ToLocation, reader["ToLocation"]);
                Assert.Equal(newTour.TransportType, reader["TransportType"]);
                Assert.Equal(newTour.Distance, (float)reader["Distance"]);
                Assert.Equal(newTour.EstimatedTime, (int)reader["EstimatedTime"]);
                Assert.Equal(newTour.RouteInformation, reader["RouteInformation"]);
            }

            // Cleanup - delete the test tour
            await tourRepo.DeleteAsync(createdTour.Id);
        }

        [Fact]
        public void TourAddedTest()
        {
            IToursService _toursService = new ToursService(new TourPlannerDbContext());
            string name = "TestName";
            string description = "TestDescription";
            string from = "Korneuburg";
            string to = "Vienna";
            string transportType = "Car";
            string id = _toursService.AddTour(name, description, from, to, transportType);
            IToursRepository tourRepo = new ToursRepository(new TourPlannerDbContext());

            Tour newTour = tourRepo.GetTourById(id);
            Assert.Equal(newTour.name, name);
        }
    }
}
