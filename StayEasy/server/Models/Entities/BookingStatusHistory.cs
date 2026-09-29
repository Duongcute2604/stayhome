using System.ComponentModel.DataAnnotations;

namespace server.Models.Entities
{
    // ============================================================================
    // BOOKING STATUS HISTORY ENTITY - Bảng lịch sử trạng thái đặt phòng
    // ============================================================================
    public class BookingStatusHistory
    {
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Note { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
