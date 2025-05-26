using TourPlanner.Models;

namespace TourPlanner.Data.Repositories
{
    public interface ITourRepository
    {
        Task<IEnumerable<Tour>> GetAllAsync();
        Task<Tour?> GetByIdAsync(string id);
        Task<Tour> CreateAsync(Tour tour);
        Task<Tour> UpdateAsync(Tour tour);
        Task DeleteAsync(string id);
        Task<IEnumerable<Tour>> GetByListIdAsync(string listId);
    }
} 