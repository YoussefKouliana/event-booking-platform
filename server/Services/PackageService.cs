using server.Models;

namespace server.Services
{
    public class PackageService
    {
        private readonly Dictionary<PackageType, PackageConfig> _packages;
        private readonly Dictionary<string, AddOnConfig> _addOns;

        public PackageService()
        {
            _packages = new Dictionary<PackageType, PackageConfig>
            {
                [PackageType.Essential] = new PackageConfig
                {
                    Name = "Essential",
                    Price = 49m,
                    MaxGuests = 50,
                    Features = new List<string>
                    {
                        "Up to 50 guests",
                        "Basic customization",
                        "RSVP tracking",
                        "Email notifications",
                        "Custom guest links"
                    },
                    AllowedAddOns = new List<string> { "qr-code", "sms-notifications", "premium-music" }
                },
                [PackageType.Professional] = new PackageConfig
                {
                    Name = "Professional",
                    Price = 89m,
                    MaxGuests = null, // Unlimited
                    Features = new List<string>
                    {
                        "Unlimited guests",
                        "Premium themes & customization",
                        "Location map integration",
                        "Music upload",
                        "RSVP management dashboard",
                        "Analytics & reporting"
                    },
                    IncludedAddOns = new List<string> { "analytics" },
                    AllowedAddOns = new List<string> { "qr-code", "guest-notes", "sms-notifications", "premium-music" }
                },
                [PackageType.Premium] = new PackageConfig
                {
                    Name = "Premium",
                    Price = 149m,
                    MaxGuests = null,
                    Features = new List<string>
                    {
                        "Everything in Professional",
                        "Guest notes & dietary preferences",
                        "Table management system",
                        "QR code check-in",
                        "Priority support",
                        "Advanced analytics"
                    },
                    IncludedAddOns = new List<string> { "qr-code", "guest-notes", "table-management", "analytics" },
                    AllowedAddOns = new List<string> { "sms-notifications", "premium-music", "custom-branding" }
                }
            };

            _addOns = new Dictionary<string, AddOnConfig>
            {
                ["qr-code"] = new AddOnConfig { Name = "QR Code Check-in", Price = 25m, Description = "Generate QR codes for easy guest check-in" },
                ["guest-notes"] = new AddOnConfig { Name = "Guest Notes & Preferences", Price = 20m, Description = "Collect dietary restrictions and special notes" },
                ["table-management"] = new AddOnConfig { Name = "Table Management System", Price = 30m, Description = "Assign guests to tables with seating charts" },
                ["sms-notifications"] = new AddOnConfig { Name = "SMS Notifications", Price = 10m, Description = "Send RSVP reminders via SMS" },
                ["premium-music"] = new AddOnConfig { Name = "Premium Music Library", Price = 15m, Description = "Access to curated wedding music collection" },
                ["custom-branding"] = new AddOnConfig { Name = "Custom Branding", Price = 30m, Description = "Add your logo and custom colors" },
                ["analytics"] = new AddOnConfig { Name = "Advanced Analytics", Price = 0m, Description = "Detailed RSVP and engagement reports" }
            };
        }

        public PackageConfig GetPackage(PackageType packageType)
        {
            return _packages[packageType];
        }

        public IEnumerable<PackageConfig> GetAllPackages()
        {
            return _packages.Values;
        }

        public AddOnConfig? GetAddOn(string addOnKey)
        {
            return _addOns.TryGetValue(addOnKey, out var addOn) ? addOn : null;
        }

        public IEnumerable<AddOnConfig> GetAvailableAddOns(PackageType packageType)
        {
            var package = GetPackage(packageType);
            return _addOns.Where(kv => package.AllowedAddOns.Contains(kv.Key))
                         .Select(kv => kv.Value);
        }

        public decimal CalculateTotalPrice(PackageType packageType, List<string>? selectedAddOns = null)
        {
            var package = GetPackage(packageType);
            var total = package.Price;

            if (selectedAddOns != null)
            {
                foreach (var addOnKey in selectedAddOns)
                {
                    if (_addOns.TryGetValue(addOnKey, out var addOn))
                    {
                        // Don't charge for included add-ons
                        if (!package.IncludedAddOns.Contains(addOnKey))
                        {
                            total += addOn.Price;
                        }
                    }
                }
            }

            return total;
        }

        public bool CanUseFeature(PackageType packageType, string feature, List<string>? userAddOns = null)
        {
            var package = GetPackage(packageType);
            
            // Check if feature is included in package
            if (package.IncludedAddOns.Contains(feature))
                return true;
                
            // Check if user has purchased the add-on
            return userAddOns?.Contains(feature) == true;
        }

        public bool IsGuestLimitExceeded(PackageType packageType, int guestCount)
        {
            var package = GetPackage(packageType);
            return package.MaxGuests.HasValue && guestCount > package.MaxGuests.Value;
        }
    }

    // Configuration classes
    public class PackageConfig
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int? MaxGuests { get; set; } // null = unlimited
        public List<string> Features { get; set; } = new();
        public List<string> IncludedAddOns { get; set; } = new(); // Free with this package
        public List<string> AllowedAddOns { get; set; } = new(); // Can be purchased
    }

    public class AddOnConfig
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}