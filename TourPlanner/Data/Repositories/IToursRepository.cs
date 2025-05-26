using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Models;

namespace TourPlanner.Data.Repositories
{
    public interface IToursRepository
    {
        public void CreateTour(Tour newTour);
        public Tour[] GetTours();
        public Tour GetTourById(string id);
        public List<Tour> GetTourByName(string name);
        public void DeleteTour(string id);
        public void ModifyTour(string currentTour, Tour newTour);
    }
}
