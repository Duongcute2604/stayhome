using System.ComponentModel.DataAnnotations;

namespace server.Models.DTOs
{
    // ============================================================================
    // LOCATION DTOs - Data Transfer Objects cho địa điểm
    // ============================================================================
    // Lưu ý: DTO giữ Id để Admin sửa/xóa. FE KHÔNG hiển thị id ra UI,
    // chỉ dùng làm key nội bộ (đúng quy tắc ẩn ID).
    // ============================================================================

    public class LocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int RoomCount { get; set; }
    }

    public class CreateLocationRequest
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }
    }

    public class UpdateLocationRequest
    {
        [MaxLength(255)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }
    }
}
