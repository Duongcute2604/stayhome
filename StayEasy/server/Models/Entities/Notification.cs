using System.ComponentModel.DataAnnotations;

namespace server.Models.Entities
{
    // ============================================================================
    // NOTIFICATION ENTITY - Bảng thông báo
    // ============================================================================
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Message { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "INFO"; // INFO, WARNING, ERROR

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
