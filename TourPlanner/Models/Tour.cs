using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourPlanner.Models
{
    public class Tour
    {
        [Key]
        public string Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [Required]
        public string FromLocation { get; set; }
        
        [Required]
        public string ToLocation { get; set; }
        
        [Required]
        public string TransportType { get; set; }
        
        public float Distance { get; set; }
        
        public int EstimatedTime { get; set; }
        
        public string RouteInformation { get; set; }
        public string? CoordsFrom { get; set; }
        public string? CoordsTo { get; set; }

        public string? ListId { get; set; }
        
        [ForeignKey("ListId")]
        public List? List { get; set; }
        
        public Tour()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
