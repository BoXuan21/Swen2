using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourPlanner.Models
{
    public class TourLog
    {
        [Key]
        public string Id { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        public string Comment { get; set; }
        
        public int Difficulty { get; set; } // 1-5 scale
        
        public int Rating { get; set; } // 1-5 scale
        
        public TimeSpan Duration { get; set; }
        
        [Required]
        public string TourId { get; set; }
        
        [ForeignKey("TourId")]
        public Tour? Tour { get; set; }
        
        public TourLog()
        {
            Id = Guid.NewGuid().ToString();
            Date = DateTime.Now;
        }
    }
} 