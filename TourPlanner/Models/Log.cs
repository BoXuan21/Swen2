using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tourplaner.backend.Models
{
    internal class Log
    {
        private string Id { get; set; }
        private string DateTime { get; set; }
        private string Comment { get; set; }
        private int Difficulty { get; set; }
        private float TotalDistance { get; set; }
        private int TotalTime { get; set; }
        private int Rating { get; set; }
        private string TourId { get; set; }

        public Log(string id, string dateTime, string comment, int difficulty, float totalDistance, int totalTime, int rating, string tourId)
        {
            this.Id = id;
            this.DateTime = dateTime;
            this.Comment = comment;
            this.Difficulty = difficulty;
            this.TotalDistance = totalDistance;
            this.TotalTime = totalTime;
            this.Rating = rating;
            this.TourId = tourId;
        }
    }
}
