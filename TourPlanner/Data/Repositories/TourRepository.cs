using Microsoft.EntityFrameworkCore;
using TourPlanner.Models;

namespace TourPlanner.Data.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly TourPlannerContext _context;

        public TourRepository(TourPlannerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tour>> GetAllAsync()
        {
            return await _context.Tours
                .Include(t => t.List)
                .Include(t => t.Logs)
                .ToListAsync();
        }

        public async Task<Tour?> GetByIdAsync(string id)
        {
            return await _context.Tours
                .Include(t => t.List)
                .Include(t => t.Logs)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tour> CreateAsync(Tour tour)
        {
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();
            return tour;
        }

        public async Task<Tour> UpdateAsync(Tour tour)
        {
            _context.Entry(tour).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return tour;
        }

        public async Task DeleteAsync(string id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour != null)
            {
                _context.Tours.Remove(tour);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Tour>> GetByListIdAsync(string listId)
        {
            return await _context.Tours
                .Include(t => t.List)
                .Include(t => t.Logs)
                .Where(t => t.ListId == listId)
                .ToListAsync();
        }
    }
} 