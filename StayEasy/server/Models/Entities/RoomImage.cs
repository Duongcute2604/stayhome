using System.ComponentModel.DataAnnotations;

namespace server.Models.Entities
{
    // ============================================================================
    // ROOM IMAGE ENTITY - Bảng hình ảnh phòng
    // ============================================================================
    public class RoomImage
    {
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public Room? Room { get; set; }
    }
}
