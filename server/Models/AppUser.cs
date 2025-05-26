using Microsoft.AspNetCore.Identity;

namespace server.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<Booking>? Bookings { get; set; }
    }
}
