using TourPlaner.Models;
using TourPlanner.Data;
using TourPlanner.Data.Repositories;
using TourPlanner.Services;
using TourPlanner.Services.Interfaces;

namespace TourPlanner.Test.RepositoryTests
{
    public class ToursTest
    {
        [Fact]
        public void TourCreatedTest()
        {
            string id = Guid.NewGuid().ToString();
            string name = "TestName";
            string description = "TestDescription";
            string from = "Korneuburg";
            string to = "Vienna";
            string transportType = "Car";
            float distance = 16.0f;
            int estimatedTime = 200;
            string routeInformation = "testPic";
            Tour newTour = new Tour(id, name, description, from, to, transportType, distance, estimatedTime, routeInformation);

            IToursRepository tourRepo = new ToursRepository(new TourPlannerDbContext());

            tourRepo.CreateTour(newTour);

            Assert.Equal(newTour, tourRepo.GetTourById(id));
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
