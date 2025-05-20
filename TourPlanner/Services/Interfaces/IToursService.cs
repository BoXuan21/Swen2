using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Services.Interfaces
{
    public interface IToursService
    {
        public string AddTour(string name, string description, string from, string to, string transportType);
    }
}
