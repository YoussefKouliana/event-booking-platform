namespace server.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    }
}
