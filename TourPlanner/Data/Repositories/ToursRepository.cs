using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Models;
using TourPlanner.Data;
using Microsoft.EntityFrameworkCore;

namespace TourPlanner.Data.Repositories
{
    public class ToursRepository : IToursRepository
    {
        private readonly TourPlannerContext _context;
        public ToursRepository(TourPlannerContext context)
        {
            _context = context;
        }
        public void CreateTour(Tour newTour)
        {
            _context.Tours.Add(newTour);
            _context.SaveChanges();
        }

        public void DeleteTour(string id)
        {
            var tour = _context.Tours.Find(id);
            if (tour != null)
            {
                _context.Tours.Remove(tour);
                _context.SaveChanges();
            }
        }

        public Tour GetTourById(string id)
        {
            return _context.Tours
                .Include(t => t.Logs)
                .Include(t => t.List)
                .FirstOrDefault(t => t.Id == id);
        }

        public Tour[] GetTours()
        {
            return _context.Tours
                .Include(t => t.Logs)
                .Include(t => t.List)
                .ToArray();
        }

        public void ModifyTour(string currentTourId, Tour newTour)
        {
            var existingTour = _context.Tours.Find(currentTourId);
            if (existingTour != null)
            {
                // Update properties
                existingTour.Name = newTour.Name;
                existingTour.Description = newTour.Description;
                existingTour.FromLocation = newTour.FromLocation;
                existingTour.ToLocation = newTour.ToLocation;
                existingTour.TransportType = newTour.TransportType;
                existingTour.Distance = newTour.Distance;
                existingTour.EstimatedTime = newTour.EstimatedTime;
                existingTour.RouteInformation = newTour.RouteInformation;
                existingTour.ListId = newTour.ListId;

                _context.SaveChanges();
            }
        }
    }
}
