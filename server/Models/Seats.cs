using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Seat
    {
        public int Id { get; set; }
        
        [Required]
        public int TableId { get; set; }
        
        public int? GuestId { get; set; } // Nullable for unassigned seats
        
        [MaxLength(10)]
        public string? SeatNumber { get; set; } // "1", "A", "Host", etc.
        
        [MaxLength(100)]
        public string? SeatLabel { get; set; } // "Host Seat", "Bride's Mother", etc.
        
        public bool IsReserved { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public Table Table { get; set; } = null!;
        public Guest? Guest { get; set; }
    }
}