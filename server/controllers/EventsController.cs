using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using server.Data;
using server.Models;
using server.DTOs;
using server.Services;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PackageService _packageService; // 🆕 Add this

        public EventsController(AppDbContext context, PackageService packageService) // 🆕 Add parameter
        {
            _context = context;
            _packageService = packageService; // 🆕 Add this
        }

        // 🆕 GET: api/events/packages - Get all available packages
        [HttpGet("packages")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<PackageDto>> GetPackages()
        {
            var packages = new[]
            {
                new { Type = PackageType.Essential, Config = _packageService.GetPackage(PackageType.Essential) },
                new { Type = PackageType.Professional, Config = _packageService.GetPackage(PackageType.Professional) },
                new { Type = PackageType.Premium, Config = _packageService.GetPackage(PackageType.Premium) }
            }
            .Select(p => new PackageDto
            {
                Id = p.Type,
                Name = p.Config.Name,
                Price = p.Config.Price,
                Features = p.Config.Features,
                MaxGuests = p.Config.MaxGuests,
                Popular = p.Config.Name == "Professional",
                IncludedAddOns = p.Config.IncludedAddOns,
                AvailableAddOns = p.Config.AllowedAddOns.Select(addOnKey => 
                {
                    var addOn = _packageService.GetAddOn(addOnKey);
                    return new AddOnDto
                    {
                        Key = addOnKey,
                        Name = addOn?.Name ?? "",
                        Price = addOn?.Price ?? 0,
                        Description = addOn?.Description ?? ""
                    };
                }).ToList()
            });

            return Ok(packages);
        }

        // 🆕 POST: api/events/calculate-price - Calculate price for selected package + add-ons
        [HttpPost("calculate-price")]
        [AllowAnonymous]
        public ActionResult<PriceBreakdownDto> CalculatePrice([FromBody] PriceCalculationDto dto)
        {
            var totalPrice = _packageService.CalculateTotalPrice(dto.PackageType, dto.AddOns);
            var package = _packageService.GetPackage(dto.PackageType);
            
            var breakdown = new PriceBreakdownDto
            {
                PackagePrice = package.Price,
                TotalPrice = totalPrice,
                IncludedFeatures = package.IncludedAddOns,
                AddOnPrices = dto.AddOns?.Select(addOnKey => 
                {
                    var addOn = _packageService.GetAddOn(addOnKey);
                    return new AddOnPriceDto
                    {
                        Key = addOnKey,
                        Name = addOn?.Name ?? "",
                        Price = addOn?.Price ?? 0,
                        IsIncluded = package.IncludedAddOns.Contains(addOnKey),
                        Description = addOn?.Description ?? ""
                    };
                }).ToList() ?? new List<AddOnPriceDto>()
            };

            return Ok(breakdown);
        }

        // GET: api/events - Get all events for the authenticated user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventListDto>>> GetUserEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var events = await _context.Events
                .Include(e => e.Guests)
                .ThenInclude(g => g.Rsvps)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new EventListDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Slug = e.Slug,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    EventType = e.EventType,
                    EventTypeName = e.EventType.ToString(),
                    Theme = e.Theme,
                    CreatedAt = e.CreatedAt,
                    // 🆕 Package Information
                    PackageType = e.PackageType,
                    PackageName = _packageService.GetPackage(e.PackageType).Name,
                    IsPaid = e.IsPaid,
                    PaymentStatus = e.PaymentStatus,
                    // Stats
                    TotalGuests = e.Guests.Count,
                    ConfirmedRsvps = e.Guests.SelectMany(g => g.Rsvps).Count(r => r.Status == "Attending"),
                    IsUpcoming = e.EventDate > DateTime.UtcNow,
                    DaysUntilEvent = (int)(e.EventDate - DateTime.UtcNow).TotalDays
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/{id} - Get specific event with full details
        [HttpGet("{id}")]
        public async Task<ActionResult<EventResponseDto>> GetEvent(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var eventItem = await _context.Events
                .Include(e => e.Guests)
                .ThenInclude(g => g.Rsvps)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (eventItem == null)
                return NotFound();

            var rsvps = eventItem.Guests.SelectMany(g => g.Rsvps).ToList();
            var package = _packageService.GetPackage(eventItem.PackageType);

            var response = new EventResponseDto
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Slug = eventItem.Slug,
                EventDate = eventItem.EventDate,
                Location = eventItem.Location,
                Description = eventItem.Description,
                EventType = eventItem.EventType,
                EventTypeName = eventItem.EventType.ToString(),
                Theme = eventItem.Theme,
                CustomFields = eventItem.CustomFields,
                CreatedAt = eventItem.CreatedAt,
                UpdatedAt = eventItem.UpdatedAt,
                // 🆕 Package Information
                PackageType = eventItem.PackageType,
                PackageName = package.Name,
                PackagePrice = eventItem.PackagePrice,
                EnabledAddOns = eventItem.AddOnsList,
                TotalAmount = eventItem.TotalAmount,
                // 🆕 Payment Information
                IsPaid = eventItem.IsPaid,
                PaidAt = eventItem.PaidAt,
                PaymentStatus = eventItem.PaymentStatus,
                // Stats
                TotalGuests = eventItem.Guests.Count,
                ConfirmedRsvps = rsvps.Count(r => r.Status == "Attending"),
                PendingRsvps = rsvps.Count(r => r.Status == "Pending"),
                DeclinedRsvps = rsvps.Count(r => r.Status == "Declined"),
                // 🆕 Feature Availability
                CanUseQRCode = eventItem.CanUseFeature("qr-code"),
                CanUseGuestNotes = eventItem.CanUseFeature("guest-notes"),
                CanUseTableManagement = eventItem.CanUseFeature("table-management"),
                MaxGuests = package.MaxGuests
            };

            return Ok(response);
        }

        // GET: api/events/slug/{slug} - Get event by slug (for public access)
        [HttpGet("slug/{slug}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventResponseDto>> GetEventBySlug(string slug)
        {
            var eventItem = await _context.Events
                .Include(e => e.Guests)
                .ThenInclude(g => g.Rsvps)
                .FirstOrDefaultAsync(e => e.Slug == slug);

            if (eventItem == null)
                return NotFound();

            var rsvps = eventItem.Guests.SelectMany(g => g.Rsvps).ToList();
            var package = _packageService.GetPackage(eventItem.PackageType);

            var response = new EventResponseDto
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Slug = eventItem.Slug,
                EventDate = eventItem.EventDate,
                Location = eventItem.Location,
                Description = eventItem.Description,
                EventType = eventItem.EventType,
                EventTypeName = eventItem.EventType.ToString(),
                Theme = eventItem.Theme,
                CustomFields = eventItem.CustomFields,
                CreatedAt = eventItem.CreatedAt,
                UpdatedAt = eventItem.UpdatedAt,
                // Package info (limited for public)
                PackageType = eventItem.PackageType,
                PackageName = package.Name,
                // Stats
                TotalGuests = eventItem.Guests.Count,
                ConfirmedRsvps = rsvps.Count(r => r.Status == "Attending"),
                PendingRsvps = rsvps.Count(r => r.Status == "Pending"),
                DeclinedRsvps = rsvps.Count(r => r.Status == "Declined")
            };

            return Ok(response);
        }

        // POST: api/events - Create new event
        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> CreateEvent(CreateEventDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Validate event date is in the future
            if (dto.EventDate <= DateTime.UtcNow)
                return BadRequest("Event date must be in the future");

            // 🆕 Calculate pricing
            var totalAmount = _packageService.CalculateTotalPrice(dto.PackageType, dto.SelectedAddOns);
            var package = _packageService.GetPackage(dto.PackageType);

            // Check if slug already exists for this user
            var existingEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.Slug == dto.Slug && e.UserId == userId);
            
            if (existingEvent != null)
            {
                // Generate a unique slug
                var baseSlug = dto.Slug;
                var counter = 1;
                do
                {
                    dto.Slug = $"{baseSlug}-{counter}";
                    counter++;
                    existingEvent = await _context.Events
                        .FirstOrDefaultAsync(e => e.Slug == dto.Slug && e.UserId == userId);
                } while (existingEvent != null);
            }

            // Get the user entity to satisfy the required navigation property
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return Unauthorized("User not found");

            var eventItem = new Event
            {
                UserId = userId,
                User = user,
                Title = dto.Title,
                Slug = dto.Slug,
                EventDate = dto.EventDate,
                Location = dto.Location,
                Description = dto.Description,
                EventType = dto.EventType,
                Theme = dto.Theme,
                CustomFields = dto.CustomFields,
                // 🆕 Package Information
                PackageType = dto.PackageType,
                PackagePrice = package.Price,
                AddOnsList = dto.SelectedAddOns ?? new List<string>(),
                TotalAmount = totalAmount,
                // Timestamps
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            // Return the created event
            var response = new EventResponseDto
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Slug = eventItem.Slug,
                EventDate = eventItem.EventDate,
                Location = eventItem.Location,
                Description = eventItem.Description,
                EventType = eventItem.EventType,
                EventTypeName = eventItem.EventType.ToString(),
                Theme = eventItem.Theme,
                CustomFields = eventItem.CustomFields,
                CreatedAt = eventItem.CreatedAt,
                UpdatedAt = eventItem.UpdatedAt,
                // 🆕 Package Information
                PackageType = eventItem.PackageType,
                PackageName = package.Name,
                PackagePrice = eventItem.PackagePrice,
                EnabledAddOns = eventItem.AddOnsList,
                TotalAmount = eventItem.TotalAmount,
                IsPaid = eventItem.IsPaid,
                PaymentStatus = eventItem.PaymentStatus,
                // Stats
                TotalGuests = 0,
                ConfirmedRsvps = 0,
                PendingRsvps = 0,
                DeclinedRsvps = 0,
                // Features
                CanUseQRCode = eventItem.CanUseFeature("qr-code"),
                CanUseGuestNotes = eventItem.CanUseFeature("guest-notes"),
                CanUseTableManagement = eventItem.CanUseFeature("table-management"),
                MaxGuests = package.MaxGuests
            };

            return CreatedAtAction(nameof(GetEvent), new { id = eventItem.Id }, response);
        }

        // PUT: api/events/{id} - Update event
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, UpdateEventDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (eventItem == null)
                return NotFound();

            // Validate event date if provided
            if (dto.EventDate.HasValue && dto.EventDate <= DateTime.UtcNow)
                return BadRequest("Event date must be in the future");

            // Update only provided fields
            if (!string.IsNullOrEmpty(dto.Title))
                eventItem.Title = dto.Title;
            
            if (dto.EventDate.HasValue)
                eventItem.EventDate = dto.EventDate.Value;
            
            if (!string.IsNullOrEmpty(dto.Location))
                eventItem.Location = dto.Location;
            
            if (dto.Description != null)
                eventItem.Description = dto.Description;
            
            if (dto.EventType.HasValue)
                eventItem.EventType = dto.EventType.Value;
            
            if (!string.IsNullOrEmpty(dto.Theme))
                eventItem.Theme = dto.Theme;
            
            if (dto.CustomFields != null)
                eventItem.CustomFields = dto.CustomFields;

            // 🆕 Update package if provided (for upgrades)
            if (dto.PackageType.HasValue)
            {
                eventItem.PackageType = dto.PackageType.Value;
                var package = _packageService.GetPackage(dto.PackageType.Value);
                eventItem.PackagePrice = package.Price;
                
                if (dto.SelectedAddOns != null)
                    eventItem.AddOnsList = dto.SelectedAddOns;
                
                eventItem.TotalAmount = _packageService.CalculateTotalPrice(dto.PackageType.Value, eventItem.AddOnsList);
                // Reset payment status for package changes
                eventItem.IsPaid = false;
                eventItem.PaymentStatus = "Pending";
            }

            eventItem.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventExists(id, userId))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/events/{id} - Delete event
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (eventItem == null)
                return NotFound();

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/events/{id}/stats - Get event statistics
        [HttpGet("{id}/stats")]
        public async Task<ActionResult<object>> GetEventStats(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var eventItem = await _context.Events
                .Include(e => e.Guests)
                .ThenInclude(g => g.Rsvps)
                .Include(e => e.Tables)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (eventItem == null)
                return NotFound();

            var rsvps = eventItem.Guests.SelectMany(g => g.Rsvps).ToList();
            var totalPartySize = rsvps.Where(r => r.Status == "Attending").Sum(r => r.PartySize);
            var package = _packageService.GetPackage(eventItem.PackageType);

            var stats = new
            {
                TotalGuests = eventItem.Guests.Count,
                TotalInvited = eventItem.Guests.Count,
                ConfirmedRsvps = rsvps.Count(r => r.Status == "Attending"),
                PendingRsvps = rsvps.Count(r => r.Status == "Pending"),
                DeclinedRsvps = rsvps.Count(r => r.Status == "Declined"),
                TotalAttending = totalPartySize,
                ResponseRate = eventItem.Guests.Count > 0 ? 
                    Math.Round((double)rsvps.Count(r => r.Status != "Pending") / eventItem.Guests.Count * 100, 1) : 0,
                TablesSetup = eventItem.Tables.Count,
                DaysUntilEvent = (int)(eventItem.EventDate - DateTime.UtcNow).TotalDays,
                IsUpcoming = eventItem.EventDate > DateTime.UtcNow,
                // 🆕 Package info
                PackageName = package.Name,
                IsGuestLimitExceeded = _packageService.IsGuestLimitExceeded(eventItem.PackageType, eventItem.Guests.Count),
                MaxGuests = package.MaxGuests
            };

            return Ok(stats);
        }

        private async Task<bool> EventExists(int id, string userId)
        {
            return await _context.Events.AnyAsync(e => e.Id == id && e.UserId == userId);
        }
    }
}