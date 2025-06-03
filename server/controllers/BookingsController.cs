using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using System.Security.Claims;

namespace server.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BookingsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //  Admin-only: Get all bookings
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.User)
                .ToListAsync();

            return Ok(bookings);
        }

        //  Authenticated user: Get my bookings
        [Authorize]
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Event)
                .ToListAsync();

            return Ok(bookings);
        }

        //  Book an event
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BookEvent([FromBody] BookingRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var eventToBook = await _context.Events
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.Id == dto.EventId);

            if (eventToBook == null)
                return NotFound("Event not found.");

            //  Already booked?
            bool alreadyBooked = await _context.Bookings
                .AnyAsync(b => b.EventId == dto.EventId && b.UserId == userId);

            if (alreadyBooked)
                return BadRequest("You’ve already booked this event.");

            //  Event full?
            int bookedCount = await _context.Bookings
                .CountAsync(b => b.EventId == dto.EventId);

            if (bookedCount >= eventToBook.MaxAttendees)
                return BadRequest("This event is fully booked.");

            var booking = new Booking
            {
                EventId = dto.EventId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok("Booking successful.");
        }

        //  Cancel booking (user only their own)
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
                return NotFound("Booking not found.");

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok("Booking cancelled.");
        }
    }

    // DTO for creating a booking
    public class BookingRequestDto
    {
        public int EventId { get; set; }
    }
}
