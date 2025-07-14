using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class Payment
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required, MaxLength(50)]
        public string Plan { get; set; } = string.Empty; // Basic, Premium, Pro
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public DateTime PaidAt { get; set; }
        
        [MaxLength(100)]
        public string? StripePaymentIntentId { get; set; }
        
        [MaxLength(100)]
        public string? StripeCustomerId { get; set; }
        
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public AppUser User { get; set; } = null!;
    }
}