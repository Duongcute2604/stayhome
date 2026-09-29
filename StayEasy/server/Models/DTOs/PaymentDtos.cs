using System.ComponentModel.DataAnnotations;

namespace server.Models.DTOs
{
    // ============================================================================
    // PAYMENT DTOs
    // ============================================================================

    public class PaymentDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public decimal? RefundAmount { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PayRequest
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public string Method { get; set; } = "CASH"; // CASH, MOMO, VNPAY
    }
}
