using System.ComponentModel.DataAnnotations;

namespace server.Models.Entities
{
    // ============================================================================
    // REVIEW IMAGE ENTITY - Bảng hình ảnh đánh giá
    // ============================================================================
    public class ReviewImage
    {
        public int Id { get; set; }

        [Required]
        public int ReviewId { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public Review? Review { get; set; }
    }
}
