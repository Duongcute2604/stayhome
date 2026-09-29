using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models.Entities
{
    // ============================================================================
    // BOOKING ENTITY - Bảng đặt phòng
    // ============================================================================
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public int RoomId { get; set; }
        public Room? Room { get; set; }

        [Required]
        [MaxLength(50)]
        public string BookingType { get; set; } = "DAILY"; // HOURLY, DAILY

        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "PENDING"; // PENDING, CONFIRMED, CHECKED_IN, CHECKED_OUT, CANCELLED

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<BookingStatusHistory> StatusHistories { get; set; } = new List<BookingStatusHistory>();
    }
}
