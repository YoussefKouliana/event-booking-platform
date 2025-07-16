using System.ComponentModel.DataAnnotations;

namespace server.DTOs
{
    public class GuestDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CustomLink { get; set; } = string.Empty;
        public string? TableNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // RSVP Status
        public string RsvpStatus { get; set; } = "Pending";
        public int PartySize { get; set; } = 1;
        public string? RsvpNote { get; set; }
        public DateTime? RsvpSubmittedAt { get; set; }
    }

    public class GuestDetailDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CustomLink { get; set; } = string.Empty;
        public string? TableNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Event Information
        public string EventTitle { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string EventLocation { get; set; } = string.Empty;
        
        // RSVP Details
        public List<RsvpDto> Rsvps { get; set; } = new List<RsvpDto>();
        
        // Seating Information
        public List<SeatDto> Seats { get; set; } = new List<SeatDto>();
        
        // Invitation URL
        public string InvitationUrl { get; set; } = string.Empty;
    }

    public class CreateGuestDto
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [StringLength(10)]
        public string? TableNumber { get; set; }

        public int? AssignedTableId { get; set; }
    }

    public class UpdateGuestDto
    {
        [StringLength(200, MinimumLength = 2)]
        public string? Name { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(10)]
        public string? TableNumber { get; set; }

        public int? AssignedTableId { get; set; }
    }

    public class BulkCreateGuestsDto
    {
        [Required]
        public List<CreateGuestDto> Guests { get; set; } = new List<CreateGuestDto>();
        
        public bool SendInvitations { get; set; } = false;
        public string? DefaultTableNumber { get; set; }
    }

    public class BulkImportResultDto
    {
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<GuestDto> ImportedGuests { get; set; } = new List<GuestDto>();
        
        // Computed properties for backward compatibility
        public int SuccessfulImports => SuccessCount;
        public int FailedImports => ErrorCount;
    }

    public class RsvpDto
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public string Status { get; set; } = "Pending";
        public int PartySize { get; set; } = 1;
        public string? Note { get; set; }
        public DateTime SubmittedAt { get; set; }
    }

    public class SeatDto
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public int GuestId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsReserved { get; set; }
    }

    public class GuestListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? TableNumber { get; set; }
        public string RsvpStatus { get; set; } = "Pending";
        public int PartySize { get; set; } = 1;
        public DateTime CreatedAt { get; set; }
        public DateTime? RsvpSubmittedAt { get; set; }
        public string CustomLink { get; set; } = string.Empty;
        
        // Quick access properties
        public bool HasResponded { get; set; }
        public bool IsAttending { get; set; }
    }
}