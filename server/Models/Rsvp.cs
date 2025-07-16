using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Rsvp
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public required Guest Guest { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Attending, Declined
        
        [Range(1, 20)]
        public int PartySize { get; set; } = 1;
        
        [StringLength(1000)]
        public string? Note { get; set; }
        
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}