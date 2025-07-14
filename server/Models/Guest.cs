using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Guest
    {
        public int Id { get; set; }
        
        [Required]
        public int EventId { get; set; }
        
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string? Email { get; set; }
        
        [MaxLength(50)]
        public string? CustomLink { get; set; }
        
        [MaxLength(20)]
        public string? TableNumber { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public Event Event { get; set; } = null!;
        public ICollection<Rsvp> Rsvps { get; set; } = new List<Rsvp>();
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}