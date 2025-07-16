using System.ComponentModel.DataAnnotations;

namespace server.Models
{
     public class Media
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public required Event Event { get; set; }
        
        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string FileUrl { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? FileType { get; set; }
        
        public long? FileSize { get; set; }
        
        public bool IsPublic { get; set; } = true;
        
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }


}