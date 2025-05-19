using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TourPlanner.Models
{
    public class List
    {
        [Key]
        public string Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public ICollection<Tour> Tours { get; set; }
        
        public List()
        {
            Id = Guid.NewGuid().ToString();
            Tours = new List<Tour>();
        }
    }
}
