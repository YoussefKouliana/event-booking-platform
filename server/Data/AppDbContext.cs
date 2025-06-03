using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Title = "Tech Conference 2025",
                    Description = "A day of inspiring talks from industry leaders.",
                    Date = new DateTime(2025, 6, 20),
                    MaxAttendees = 300
                },
                new Event
                {
                    Id = 2,
                    Title = "Startup Demo Day",
                    Description = "Pitch your startup to real investors.",
                    Date = new DateTime(2025, 7, 15),
                    MaxAttendees = 150
                },
                new Event
                {
                    Id = 3,
                    Title = "React Workshop",
                    Description = "Hands-on session building modern UIs with React.",
                    Date = new DateTime(2025, 8, 5),
                    MaxAttendees = 100
                }
            );
        }

    }
}
