using System.ComponentModel.DataAnnotations;
using server.Models;

namespace server.DTOs
{
    public class CreateEventDto
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string Location { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public EventType EventType { get; set; }

        [StringLength(50)]
        public string Theme { get; set; } = string.Empty;

        public string? CustomFields { get; set; }
    }

    public class UpdateEventDto
    {
        [StringLength(200, MinimumLength = 3)]
        public string? Title { get; set; }

        public DateTime? EventDate { get; set; }

        [StringLength(500, MinimumLength = 5)]
        public string? Location { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public EventType? EventType { get; set; }

        [StringLength(50)]
        public string? Theme { get; set; }

        public string? CustomFields { get; set; }
    }

    public class EventResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public EventType EventType { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
        public string? CustomFields { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Statistics
        public int TotalGuests { get; set; }
        public int ConfirmedRsvps { get; set; }
        public int PendingRsvps { get; set; }
        public int DeclinedRsvps { get; set; }
    }

    public class EventListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public EventType EventType { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        // Quick stats
        public int TotalGuests { get; set; }
        public int ConfirmedRsvps { get; set; }
        
        // Status indicators
        public bool IsUpcoming { get; set; }
        public int DaysUntilEvent { get; set; }
    }
}