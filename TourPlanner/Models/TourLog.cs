using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourPlanner.Models
{
    public class TourLog
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
        
        [Required]
        public string Comment { get; set; }
        
        [Range(1, 5)]
        public int Difficulty { get; set; } // 1-5 scale
        
        [Range(1, 5)]
        public int Rating { get; set; } // 1-5 scale
        
        public TimeSpan Duration { get; set; }
        
        [Required]
        public string TourId { get; set; }
        
        [ForeignKey("TourId")]
        public Tour? Tour { get; set; }
    }
} 