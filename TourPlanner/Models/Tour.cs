using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tourplaner.backend.Models
{
    internal class Tour
    {
        private string Id { get; set; }
        private string Name { get; set; }
        private string Description { get; set; }
        private string From { get; set; }
        private string To { get; set; }
        private string TransportType { get; set; }
        private float Distance { get; set; }
        private int EstimatedTime { get; set; }
        private string RouteInformation { get; set; }
        private string ListId { get; set; }

        public Tour(string id, string name, string description, string from, string to, string transportType, float distance, int estimatedTime, string routeInformation, string listId)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.From = from;
            this.To = to;
            this.TransportType = transportType;
            this.Distance = distance;
            this.EstimatedTime = estimatedTime;
            this.RouteInformation = routeInformation;
            this.ListId = listId;
        }

    }
}
