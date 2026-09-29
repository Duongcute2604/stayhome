using System.ComponentModel.DataAnnotations;

namespace server.Models.DTOs
{
    // ============================================================================
    // ROOM DTOs - Data Transfer Objects cho phòng
    // ============================================================================

    // Response phòng
    public class RoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public string? LocationName { get; set; }
        public List<string> Images { get; set; } = new();
        public List<AmenityDto> Amenities { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    // Request tạo phòng
    public class CreateRoomRequest
    {
        [Required]
        public int LocationId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PricePerHour { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PricePerDay { get; set; }

        [Required]
        [Range(1, 100)]
        public int Capacity { get; set; }

        public List<string>? Images { get; set; }
        public List<int>? AmenityIds { get; set; }
    }

    // Request cập nhật phòng
    public class UpdateRoomRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? PricePerHour { get; set; }
        public decimal? PricePerDay { get; set; }
        public int? Capacity { get; set; }
        public string? Status { get; set; }
    }

    // Request tìm kiếm & lọc phòng (bind từ query string: GET /api/rooms/search?... )
    public class RoomSearchRequest
    {
        public string? Search { get; set; }
        public int? LocationId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Capacity { get; set; }
        public List<int>? AmenityIds { get; set; }
        public string? SortBy { get; set; } // price_asc, price_desc, newest
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
