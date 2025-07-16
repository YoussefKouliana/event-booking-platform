using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required AppUser User { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Plan { get; set; } = string.Empty; // Basic, Premium, Enterprise
        
        [Range(0, 999999.99)]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded
        
        [StringLength(100)]
        public string? StripePaymentId { get; set; }
        
        [StringLength(100)]
        public string? StripeSessionId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
    }
}