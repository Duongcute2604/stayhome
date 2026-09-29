using System.ComponentModel.DataAnnotations;

namespace server.Models.DTOs
{
    // ============================================================================
    // REVIEW DTOs - Data Transfer Objects cho đánh giá
    // ============================================================================

    public class ReviewDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int RoomId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateReviewRequest
    {
        [Required]
        public int RoomId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(2000)]
        public string? Comment { get; set; }
    }
}
