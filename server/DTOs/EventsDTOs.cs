namespace server.DTOs
{
    // Event DTOs
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? Theme { get; set; }
        public string? MusicUrl { get; set; }
        public bool IsPublic { get; set; }
        public int GuestCount { get; set; }
        public int RsvpCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EventDetailDto : EventDto
    {
        public List<GuestDto> Guests { get; set; } = new();
        public List<TableDto> Tables { get; set; } = new();
    }

    public class PublicEventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? Theme { get; set; }
        public string? MusicUrl { get; set; }
    }

    public class CreateEventDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? Theme { get; set; }
        public bool IsPublic { get; set; } = true;
    }

    public class UpdateEventDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? Theme { get; set; }
        public string? MusicUrl { get; set; }
        public bool IsPublic { get; set; }
    }

    // Guest DTOs
    public class GuestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? CustomLink { get; set; }
        public string? TableNumber { get; set; }
        public string RsvpStatus { get; set; } = "Pending";
    }

    public class GuestDetailDto : GuestDto
    {
        public List<RsvpDto> Rsvps { get; set; } = new();
    }

    public class CreateGuestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? TableNumber { get; set; }
    }

    public class UpdateGuestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? TableNumber { get; set; }
    }

    public class BulkCreateGuestsDto
    {
        public List<CreateGuestDto> Guests { get; set; } = new();
    }

    public class BulkImportResultDto
    {
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    // Table DTOs
    public class TableDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int AssignedGuests { get; set; }
    }

    // RSVP DTOs
    public class RsvpDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public int PartySize { get; set; }
        public string? Note { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}