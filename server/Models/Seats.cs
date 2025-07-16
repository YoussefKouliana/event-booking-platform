using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public required Table Table { get; set; }

        public int? GuestId { get; set; }
        public Guest? Guest { get; set; }

        public bool IsReserved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}