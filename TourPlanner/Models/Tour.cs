using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlaner.Models
{
    public class Tour
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string fromlocation { get; set; }
        public string tolocation { get; set; }
        public string transporttype { get; set; }
        public float distance { get; set; }
        public int estimatedtime { get; set; }
        public string routeinformation { get; set; }
        public string? listid { get; set; }

        public Tour(string id, string name, string description, string fromlocation, string tolocation, string transporttype, float distance, int estimatedtime, string routeinformation)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.fromlocation = fromlocation;
            this.tolocation = tolocation;
            this.transporttype = transporttype;
            this.distance = distance;
            this.estimatedtime = estimatedtime;
            this.routeinformation = routeinformation;
        }

    }
}
