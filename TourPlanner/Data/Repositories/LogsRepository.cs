using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourPlanner.Models;
using TourPlanner.Data;

namespace TourPlanner.Data.Repositories
{
    public class LogsRepository : ILogsRepository
    {
        private readonly TourPlannerContext _context;

        public LogsRepository(TourPlannerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TourLog>> GetLogsByTourIdAsync(string tourId)
        {
            return await _context.Logs
                .Where(l => l.TourId == tourId)
                .OrderByDescending(l => l.Date)
                .ToListAsync();
        }

        public async Task<TourLog> GetLogByIdAsync(string id)
        {
            return await _context.Logs
                .Include(l => l.Tour)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<TourLog> CreateLogAsync(TourLog log)
        {
            try
            {
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                return log;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating log: {ex.Message}", ex);
            }
        }

        public async Task<TourLog> UpdateLogAsync(TourLog log)
        {
            try
            {
                var existingLog = await _context.Logs.FindAsync(log.Id);
                if (existingLog != null)
                {
                    existingLog.Date = log.Date;
                    existingLog.Comment = log.Comment;
                    existingLog.Difficulty = log.Difficulty;
                    existingLog.Rating = log.Rating;
                    existingLog.Duration = log.Duration;

                    await _context.SaveChangesAsync();
                    return existingLog;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating log: {ex.Message}", ex);
            }
        }

        public async Task DeleteLogAsync(string id)
        {
            var log = await _context.Logs.FindAsync(id);
            if (log != null)
            {
                _context.Logs.Remove(log);
                await _context.SaveChangesAsync();
            }
        }
    }
}
