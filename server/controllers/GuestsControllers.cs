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
    [Route("api/events/{eventId}/guests")]
    [Authorize]
    public class GuestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GuestsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/events/{eventId}/guests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuestDto>>> GetGuests(int eventId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            // Verify user owns the event
            var eventExists = await _context.Events
                .AnyAsync(e => e.Id == eventId && e.UserId == userId);
            
            if (!eventExists) return NotFound("Event not found");

            var guests = await _context.Guests
                .Where(g => g.EventId == eventId)
                .Include(g => g.Rsvps)
                .Select(g => new GuestDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Email = g.Email,
                    CustomLink = g.CustomLink,
                    TableNumber = g.TableNumber,
                    RsvpStatus = g.Rsvps.OrderByDescending(r => r.SubmittedAt)
                                       .FirstOrDefault() != null ? 
                                       g.Rsvps.OrderByDescending(r => r.SubmittedAt)
                                              .First().Status : "Pending"
                })
                .ToListAsync();

            return Ok(guests);
        }

        // GET: api/events/{eventId}/guests/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GuestDetailDto>> GetGuest(int eventId, int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var guest = await _context.Guests
                .Include(g => g.Event)
                .Include(g => g.Rsvps)
                .FirstOrDefaultAsync(g => g.Id == id && g.EventId == eventId && g.Event.UserId == userId);

            if (guest == null) return NotFound();

            var guestDetail = new GuestDetailDto
            {
                Id = guest.Id,
                Name = guest.Name,
                Email = guest.Email,
                CustomLink = guest.CustomLink,
                TableNumber = guest.TableNumber,
                Rsvps = guest.Rsvps.Select(r => new RsvpDto
                {
                    Id = r.Id,
                    Status = r.Status,
                    PartySize = r.PartySize,
                    Note = r.Note,
                    SubmittedAt = r.SubmittedAt
                }).OrderByDescending(r => r.SubmittedAt).ToList()
            };

            return Ok(guestDetail);
        }

        // POST: api/events/{eventId}/guests
        [HttpPost]
        public async Task<ActionResult<GuestDto>> CreateGuest(int eventId, CreateGuestDto createGuestDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            // Verify user owns the event
            var eventExists = await _context.Events
                .AnyAsync(e => e.Id == eventId && e.UserId == userId);
            
            if (!eventExists) return NotFound("Event not found");

            // Generate unique custom link
            var customLink = Guid.NewGuid().ToString("N")[..8];
            while (await _context.Guests.AnyAsync(g => g.CustomLink == customLink))
            {
                customLink = Guid.NewGuid().ToString("N")[..8];
            }

            var guest = new Guest
            {
                EventId = eventId,
                Name = createGuestDto.Name,
                Email = createGuestDto.Email,
                CustomLink = customLink,
                TableNumber = createGuestDto.TableNumber
            };

            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();

            var guestDto = new GuestDto
            {
                Id = guest.Id,
                Name = guest.Name,
                Email = guest.Email,
                CustomLink = guest.CustomLink,
                TableNumber = guest.TableNumber,
                RsvpStatus = "Pending"
            };

            return CreatedAtAction(nameof(GetGuest), 
                new { eventId = eventId, id = guest.Id }, guestDto);
        }

        // POST: api/events/{eventId}/guests/bulk
        [HttpPost("bulk")]
        public async Task<ActionResult<BulkImportResultDto>> BulkCreateGuests(
            int eventId, BulkCreateGuestsDto bulkCreateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            // Verify user owns the event
            var eventExists = await _context.Events
                .AnyAsync(e => e.Id == eventId && e.UserId == userId);
            
            if (!eventExists) return NotFound("Event not found");

            var results = new BulkImportResultDto
            {
                SuccessCount = 0,
                ErrorCount = 0,
                Errors = new List<string>()
            };

            foreach (var guestData in bulkCreateDto.Guests)
            {
                try
                {
                    // Generate unique custom link
                    var customLink = Guid.NewGuid().ToString("N")[..8];
                    while (await _context.Guests.AnyAsync(g => g.CustomLink == customLink))
                    {
                        customLink = Guid.NewGuid().ToString("N")[..8];
                    }

                    var guest = new Guest
                    {
                        EventId = eventId,
                        Name = guestData.Name,
                        Email = guestData.Email,
                        CustomLink = customLink,
                        TableNumber = guestData.TableNumber
                    };

                    _context.Guests.Add(guest);
                    results.SuccessCount++;
                }
                catch (Exception ex)
                {
                    results.ErrorCount++;
                    results.Errors.Add($"Error importing {guestData.Name}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
            return Ok(results);
        }

        // PUT: api/events/{eventId}/guests/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuest(int eventId, int id, UpdateGuestDto updateGuestDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var guest = await _context.Guests
                .Include(g => g.Event)
                .FirstOrDefaultAsync(g => g.Id == id && g.EventId == eventId && g.Event.UserId == userId);

            if (guest == null) return NotFound();

            guest.Name = updateGuestDto.Name;
            guest.Email = updateGuestDto.Email;
            guest.TableNumber = updateGuestDto.TableNumber;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Guests.Any(g => g.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/events/{eventId}/guests/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuest(int eventId, int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var guest = await _context.Guests
                .Include(g => g.Event)
                .FirstOrDefaultAsync(g => g.Id == id && g.EventId == eventId && g.Event.UserId == userId);

            if (guest == null) return NotFound();

            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
