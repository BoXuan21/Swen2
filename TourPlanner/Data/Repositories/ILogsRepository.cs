using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlanner.Models;

namespace TourPlanner.Data.Repositories
{
    public interface ILogsRepository
    {
        Task<IEnumerable<TourLog>> GetLogsByTourIdAsync(string tourId);
        Task<TourLog> GetLogByIdAsync(string id);
        Task<TourLog> CreateLogAsync(TourLog log);
        Task<TourLog> UpdateLogAsync(TourLog log);
        Task DeleteLogAsync(string id);
    }
}
