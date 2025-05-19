using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlaner.Models;

namespace TourPlanner.Data.Repositories
{
    public class ToursRepository : IToursRepository
    {
        TourPlannerDbContext _context;
        public ToursRepository(TourPlannerDbContext context)
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
            throw new NotImplementedException();
        }

        public Tour GetTourById(string id)
        {
            var tour = _context.Tours.Find(id);
            return tour;

        }

        public Tour[] GetTours()
        {
            throw new NotImplementedException();
        }

        public void ModifyTour(string currentTour, Tour newTour)
        {
            throw new NotImplementedException();
        }
    }
}
