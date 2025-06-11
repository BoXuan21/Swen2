using TourPlanner.Models;

namespace TourPlanner.Services
{
    public interface IToursService
    {
        public string[] ResolveCoords(Tour tour);
        public float CalculateDistance(Tour tour);
        public int CalculateEstimatedTime(Tour tour);
    }
}
