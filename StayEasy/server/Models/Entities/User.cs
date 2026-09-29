using System.ComponentModel.DataAnnotations;

namespace server.Models.Entities
{
    // ============================================================================
    // USER ENTITY - Bảng người dùng
    // ============================================================================
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Customer"; // Customer, Employee, Admin

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
