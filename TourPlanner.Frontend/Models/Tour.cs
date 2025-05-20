using System;

namespace TourPlanner.Frontend.Models
{
    public class Tour
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string TransportType { get; set; }
        public double Distance { get; set; }
        public TimeSpan EstimatedTime { get; set; }
        public string RouteImagePath { get; set; }
    }
} 