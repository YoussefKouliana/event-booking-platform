using Microsoft.AspNetCore.Identity;

namespace server.Models
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}