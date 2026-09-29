using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models.Entities
{
    // ============================================================================
    // PAYMENT ENTITY - Bảng thanh toán (MOCK, không tích hợp cổng thật)
    // Quy tắc hoàn tiền khi hủy: trước 24h check-in hoàn 100%, trong 24h hoàn 50%
    // ============================================================================
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "PAID"; // PENDING, PAID, REFUNDED

        [Required]
        [MaxLength(50)]
        public string Method { get; set; } = "CASH"; // CASH, MOMO, VNPAY (mock)

        [MaxLength(255)]
        public string? TransactionId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RefundAmount { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
