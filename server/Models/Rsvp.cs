using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Rsvp
    {
        public int Id { get; set; }
        
        [Required]
        public int GuestId { get; set; }
        
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Attending, Declined
        
        [Range(0, 20)]
        public int PartySize { get; set; } = 1;
        
        [MaxLength(1000)]
        public string? Note { get; set; }
        
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Guest Guest { get; set; } = null!;
    }
}