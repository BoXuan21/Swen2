using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlaner.Models
{
    public class Log
    {
        public string Id { get; set; }
        public string DateTime { get; set; }
        public string Comment { get; set; }
        public int Difficulty { get; set; }
        public float TotalDistance { get; set; }
        public int TotalTime { get; set; }
        public int Rating { get; set; }
        public string TourId { get; set; }

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
