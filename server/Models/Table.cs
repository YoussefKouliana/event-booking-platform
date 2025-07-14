using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Table
    {
        public int Id { get; set; }
        
        [Required]
        public int EventId { get; set; }
        
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty; // "Table 1", "VIP Table", "Family Table"
        
        [Range(1, 20)]
        public int Capacity { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [MaxLength(50)]
        public string? Shape { get; set; } // Round, Rectangle, Square
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public Event Event { get; set; } = null!;
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}