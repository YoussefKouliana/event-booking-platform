using System.ComponentModel.DataAnnotations;

namespace server.Models
{
   public class Table
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public required Event Event { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Range(1, 50)]
        public int Capacity { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation Properties
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}