using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Media
    {
        public int Id { get; set; }
        
        [Required]
        public int EventId { get; set; }
        
        [Required, MaxLength(255)]
        public string FileName { get; set; } = string.Empty;
        
        [Required, MaxLength(500)]
        public string FileUrl { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? FileType { get; set; } // image, audio, video, document
        
        [MaxLength(10)]
        public string? FileExtension { get; set; } // jpg, png, mp3, mp4, etc.
        
        public long FileSize { get; set; } // Size in bytes
        
        [MaxLength(100)]
        public string? MimeType { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public bool IsPublic { get; set; } = true;
        
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Event Event { get; set; } = null!;
    }
}