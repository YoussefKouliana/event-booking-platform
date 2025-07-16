using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Event
    {
        public int Id { get; set; }
        public required string UserId { get; set; }  // Foreign key to AppUser
        public required AppUser User { get; set; }   // Navigation property

        // Core Event Information (Universal)
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Event Type & Customization
        public EventType EventType { get; set; }  // Enum: Wedding, Baptism, etc.
        public string Theme { get; set; } = string.Empty;  // Color theme, style
        public string MusicUrl { get; set; } = string.Empty;

        // Event-Specific Settings (JSON for flexibility)
        public string? CustomFields { get; set; }  // JSON for event-specific data

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public ICollection<Guest> Guests { get; set; } = new List<Guest>();
        public ICollection<Media> Media { get; set; } = new List<Media>();
        public ICollection<Table> Tables { get; set; } = new List<Table>();
    }

    public enum EventType
    {
        Wedding = 1,
        Engagement = 2,
        Baptism = 3,
        FirstCommunion = 4,
        Quinceañera = 5,
        BarMitzvah = 6,
        BatMitzvah = 7,
        BirthdayParty = 8,
        Anniversary = 9,
        Graduation = 10,
        BabyShower = 11,
        BridalShower = 12,
        Other = 99
    }
}