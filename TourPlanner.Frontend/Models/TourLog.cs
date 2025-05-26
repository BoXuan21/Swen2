using System;

namespace TourPlanner.Frontend.Models
{
    public class TourLog
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public int Difficulty { get; set; }
        public int Rating { get; set; }
        public TimeSpan Duration { get; set; }
        public string TourId { get; set; }
    }
} 