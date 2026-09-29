using System.ComponentModel.DataAnnotations;

namespace server.Models.DTOs
{
    // ============================================================================
    // BOOKING DTOs - Data Transfer Objects cho đặt phòng
    // ============================================================================

    // Response đặt phòng
    public class BookingDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int RoomId { get; set; }
        public string? RoomName { get; set; }
        public string BookingType { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // Request tạo đặt phòng
    public class CreateBookingRequest
    {
        [Required]
        public int RoomId { get; set; }

        [Required]
        public string BookingType { get; set; } = "DAILY"; // HOURLY, DAILY

        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }
    }

    // Request cập nhật trạng thái đặt phòng
    public class UpdateBookingStatusRequest
    {
        [Required]
        public string Status { get; set; } = string.Empty; // PENDING, CONFIRMED, CHECKED_IN, CHECKED_OUT, CANCELLED
    }
}
