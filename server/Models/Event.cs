using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace server.Models
{
    public class Event
    {
        public int Id { get; set; }
        public required string UserId { get; set; }  // Foreign key to AppUser
        public required AppUser User { get; set; }   // Navigation property

        // Core Event Information (Universal)
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Event Type & Customization
        public EventType EventType { get; set; }  // Enum: Wedding, Baptism, etc.
        public string Theme { get; set; } = string.Empty;  // Color theme, style
        public string MusicUrl { get; set; } = string.Empty;

        // Event-Specific Settings (JSON for flexibility)
        public string? CustomFields { get; set; }  // JSON for event-specific data

        // 🆕 Package & Payment Information
        [Required]
        public PackageType PackageType { get; set; } = PackageType.Essential;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PackagePrice { get; set; } = 0;
        
        // Store selected add-ons as JSON array (e.g., ["qr-code", "guest-notes"])
        public string? EnabledAddOns { get; set; } = "[]";
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } = 0;
        
        // Payment Status
        public bool IsPaid { get; set; } = false;
        public DateTime? PaidAt { get; set; }
        public string? PaymentId { get; set; }  // Stripe payment ID
        public string? PaymentSessionId { get; set; }  // Stripe session ID
        
        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public ICollection<Guest> Guests { get; set; } = new List<Guest>();
        public ICollection<Media> Media { get; set; } = new List<Media>();
        public ICollection<Table> Tables { get; set; } = new List<Table>();
        
        // 🆕 Helper Properties for Add-ons (NotMapped - not stored in database)
        [NotMapped]
        public List<string> AddOnsList 
        { 
            get 
            {
                if (string.IsNullOrEmpty(EnabledAddOns))
                    return new List<string>();
                
                try 
                {
                    return JsonSerializer.Deserialize<List<string>>(EnabledAddOns) ?? new List<string>();
                }
                catch 
                {
                    return new List<string>();
                }
            }
            set 
            {
                EnabledAddOns = JsonSerializer.Serialize(value ?? new List<string>());
            }
        }
        
        // 🆕 Helper Methods (NO [NotMapped] attribute needed for methods)
        public bool HasAddOn(string addOnKey) 
        {
            return AddOnsList.Contains(addOnKey);
        }
        
        public bool CanUseFeature(string featureKey)
        {
            // Check if feature is included in package or purchased as add-on
            return HasAddOn(featureKey) || IsFeatureIncludedInPackage(featureKey);
        }
        
        private bool IsFeatureIncludedInPackage(string featureKey)
        {
            return PackageType switch
            {
                PackageType.Essential => false, // No included add-ons
                PackageType.Professional => featureKey == "analytics",
                PackageType.Premium => featureKey is "analytics" or "qr-code" or "guest-notes" or "table-management",
                _ => false
            };
        }
    }

    public enum EventType
    {
        Wedding = 1,
        Engagement = 2,
        Baptism = 3,
        FirstCommunion = 4,
        Quinceañera = 5,
        BarMitzvah = 6,
        BatMitzvah = 7,
        BirthdayParty = 8,
        Anniversary = 9,
        Graduation = 10,
        BabyShower = 11,
        BridalShower = 12,
        Other = 99
    }

    // 🆕 Package Type Enum
    public enum PackageType
    {
        Essential = 1,
        Professional = 2,
        Premium = 3
    }
}