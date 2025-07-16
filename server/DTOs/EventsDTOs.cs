using System.ComponentModel.DataAnnotations;
using server.Models;

namespace server.DTOs
{
    public class CreateEventDto
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string Location { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public EventType EventType { get; set; }

        [StringLength(50)]
        public string Theme { get; set; } = string.Empty;

        public string? CustomFields { get; set; }

        // 🆕 Package Information
        [Required]
        public PackageType PackageType { get; set; } = PackageType.Essential;
        
        public List<string>? SelectedAddOns { get; set; } = new();
    }

    public class UpdateEventDto
    {
        [StringLength(200, MinimumLength = 3)]
        public string? Title { get; set; }

        public DateTime? EventDate { get; set; }

        [StringLength(500, MinimumLength = 5)]
        public string? Location { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public EventType? EventType { get; set; }

        [StringLength(50)]
        public string? Theme { get; set; }

        public string? CustomFields { get; set; }

        // 🆕 Package Information (for upgrades)
        public PackageType? PackageType { get; set; }
        public List<string>? SelectedAddOns { get; set; }
    }

    public class EventResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public EventType EventType { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
        public string? CustomFields { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // 🆕 Package Information
        public PackageType PackageType { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public decimal PackagePrice { get; set; }
        public List<string> EnabledAddOns { get; set; } = new();
        public decimal TotalAmount { get; set; }
        
        // 🆕 Payment Information
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        
        // Statistics
        public int TotalGuests { get; set; }
        public int ConfirmedRsvps { get; set; }
        public int PendingRsvps { get; set; }
        public int DeclinedRsvps { get; set; }
        
        // 🆕 Feature Availability (computed)
        public bool CanUseQRCode { get; set; }
        public bool CanUseGuestNotes { get; set; }
        public bool CanUseTableManagement { get; set; }
        public int? MaxGuests { get; set; }
    }

    public class EventListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public EventType EventType { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        // 🆕 Package Information
        public PackageType PackageType { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        
        // Quick stats
        public int TotalGuests { get; set; }
        public int ConfirmedRsvps { get; set; }
        
        // Status indicators
        public bool IsUpcoming { get; set; }
        public int DaysUntilEvent { get; set; }
    }

    // 🆕 New DTOs for pricing and packages
    public class PriceCalculationDto
    {
        public PackageType PackageType { get; set; }
        public List<string> AddOns { get; set; } = new();
    }

    public class PriceBreakdownDto
    {
        public decimal PackagePrice { get; set; }
        public List<AddOnPriceDto> AddOnPrices { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public List<string> IncludedFeatures { get; set; } = new();
    }

    public class AddOnPriceDto
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsIncluded { get; set; } // Free with current package
        public string Description { get; set; } = string.Empty;
    }

    public class PackageDto
    {
        public PackageType Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<string> Features { get; set; } = new();
        public int? MaxGuests { get; set; }
        public bool Popular { get; set; }
        public List<AddOnDto> AvailableAddOns { get; set; } = new();
        public List<string> IncludedAddOns { get; set; } = new();
    }

    public class AddOnDto
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}