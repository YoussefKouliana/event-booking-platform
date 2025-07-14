using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using server.DTOs;
using System.Security.Claims;

namespace server.Controllers
{
    [ApiController]
    [Route("api/events")]
    [Authorize] // Require authentication for all endpoints
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/events - Get current user's events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var events = await _context.Events
                .Where(e => e.UserId == userId)
                .Include(e => e.Guests)
                .Include(e => e.Media)
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Slug = e.Slug,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    MusicUrl = e.MusicUrl,
                    Description = e.Description,
                    Theme = e.Theme,
                    IsPublic = e.IsPublic,
                    GuestCount = e.Guests.Count,
                    RsvpCount = e.Guests.Count(g => g.Rsvps.Any(r => r.Status == "Attending")),
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EventDetailDto>> GetEvent(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var evt = await _context.Events
                .Include(e => e.Guests)
                    .ThenInclude(g => g.Rsvps)
                .Include(e => e.Media)
                .Include(e => e.Tables)
                    .ThenInclude(t => t.Seats)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (evt == null) return NotFound();

            var eventDetail = new EventDetailDto
            {
                Id = evt.Id,
                Title = evt.Title,
                Slug = evt.Slug,
                EventDate = evt.EventDate,
                Location = evt.Location,
                MusicUrl = evt.MusicUrl,
                Description = evt.Description,
                Theme = evt.Theme,
                IsPublic = evt.IsPublic,
                Guests = evt.Guests.Select(g => new GuestDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Email = g.Email,
                    CustomLink = g.CustomLink,
                    TableNumber = g.TableNumber,
                    RsvpStatus = g.Rsvps.LastOrDefault()?.Status ?? "Pending"
                }).ToList(),
                Tables = evt.Tables.Select(t => new TableDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Capacity = t.Capacity,
                    AssignedGuests = t.Seats.Count
                }).ToList(),
                CreatedAt = evt.CreatedAt
            };

            return Ok(eventDetail);
        }

        // GET: api/events/slug/{slug} - Public endpoint for guest access
        [AllowAnonymous]
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<PublicEventDto>> GetEventBySlug(string slug)
        {
            var evt = await _context.Events
                .Include(e => e.Media)
                .FirstOrDefaultAsync(e => e.Slug == slug && e.IsPublic);

            if (evt == null) return NotFound("Event not found or private");

            var publicEvent = new PublicEventDto
            {
                Id = evt.Id,
                Title = evt.Title,
                EventDate = evt.EventDate,
                Location = evt.Location,
                Description = evt.Description,
                Theme = evt.Theme,
                MusicUrl = evt.MusicUrl
            };

            return Ok(publicEvent);
        }

        // POST: api/events
        [HttpPost]
        public async Task<ActionResult<EventDto>> CreateEvent(CreateEventDto createEventDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            // Generate unique slug
            var baseSlug = createEventDto.Title.ToLower()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace("&", "and");
            
            var slug = baseSlug;
            var counter = 1;
            while (await _context.Events.AnyAsync(e => e.Slug == slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            var evt = new Event
            {
                UserId = userId,
                Title = createEventDto.Title,
                Slug = slug,
                EventDate = createEventDto.EventDate,
                Location = createEventDto.Location,
                Description = createEventDto.Description,
                Theme = createEventDto.Theme,
                IsPublic = createEventDto.IsPublic,
                CreatedAt = DateTime.UtcNow
            };

            _context.Events.Add(evt);
            await _context.SaveChangesAsync();

            var eventDto = new EventDto
            {
                Id = evt.Id,
                Title = evt.Title,
                Slug = evt.Slug,
                EventDate = evt.EventDate,
                Location = evt.Location,
                Description = evt.Description,
                Theme = evt.Theme,
                IsPublic = evt.IsPublic,
                GuestCount = 0,
                RsvpCount = 0,
                CreatedAt = evt.CreatedAt
            };

            return CreatedAtAction(nameof(GetEvent), new { id = evt.Id }, eventDto);
        }

        // PUT: api/events/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, UpdateEventDto updateEventDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var evt = await _context.Events.FindAsync(id);
            if (evt == null || evt.UserId != userId) return NotFound();

            evt.Title = updateEventDto.Title;
            evt.EventDate = updateEventDto.EventDate;
            evt.Location = updateEventDto.Location;
            evt.Description = updateEventDto.Description;
            evt.Theme = updateEventDto.Theme;
            evt.IsPublic = updateEventDto.IsPublic;
            evt.MusicUrl = updateEventDto.MusicUrl;
            evt.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Events.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/events/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var evt = await _context.Events.FindAsync(id);
            if (evt == null || evt.UserId != userId) return NotFound();

            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}