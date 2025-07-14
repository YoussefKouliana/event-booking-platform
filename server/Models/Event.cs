using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Event
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required, MaxLength(100)]
        public string Slug { get; set; } = string.Empty;
        
        public DateTime EventDate { get; set; }
        
        [MaxLength(500)]
        public string? Location { get; set; }
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [MaxLength(500)]
        public string? MusicUrl { get; set; }
        
        [MaxLength(50)]
        public string? Theme { get; set; }
        
        public bool IsPublic { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public AppUser User { get; set; } = null!;
        public ICollection<Guest> Guests { get; set; } = new List<Guest>();
        public ICollection<Media> Media { get; set; } = new List<Media>();
        public ICollection<Table> Tables { get; set; } = new List<Table>();
    }
}