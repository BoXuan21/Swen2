using TourPlaner.Models;
using TourPlanner.Data;
using TourPlanner.Data.Repositories;

namespace TourPlanner.Test.RepositoryTests
{
    public class ToursTest
    {
        [Fact]
        public void TourCreatedTest()
        {
            string id = new Guid().ToString();
            string name = "TestName";
            string description = "TestDescription";
            string from = "Korneuburg";
            string to = "Vienna";
            string transportType = "Car";
            float distance = 16.0f;
            int estimatedTime = 200;
            string routeInformation = "testPic";
            Tour newTour = new Tour(id, name, description, from, to, transportType, distance, estimatedTime, routeInformation, null);

            IToursRepository tourRepo = new ToursRepository(new TourPlannerDbContext());

            tourRepo.CreateTour(newTour);

            Assert.Equal(newTour, tourRepo.GetTourById(id));
        }
    }
}
