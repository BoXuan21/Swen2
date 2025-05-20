using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlaner.Models;
using TourPlanner.Data;
using TourPlanner.Data.Repositories;
using TourPlanner.Services.Interfaces;

namespace TourPlanner.Services
{
    public class ToursService : IToursService
    {
        TourPlannerDbContext _dbContext;
        IToursRepository _toursRepository;

        public ToursService(TourPlannerDbContext context)
        {
            _dbContext = context;
            _toursRepository = new ToursRepository(_dbContext);
        }
        public string AddTour(string name, string description, string from, string to, string transportType)
        {
            // TODO: calculate distance, time, create image instead of fixed values
            string id = Guid.NewGuid().ToString();
            float distance = 5.0f;
            int time = 200;
            string image = "";

            Tour newTour = new Tour(id, name, description, from, to, transportType, distance, time, image);

            _toursRepository.CreateTour(newTour);
            return id;
        }
    }   
}
