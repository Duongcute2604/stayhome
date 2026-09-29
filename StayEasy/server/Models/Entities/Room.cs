using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models.Entities
{
    // ============================================================================
    // ROOM ENTITY - Bảng phòng
    // ============================================================================
    public class Room
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerHour { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerDay { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "AVAILABLE"; // AVAILABLE, OCCUPIED, CLEANING, MAINTENANCE

        [Required]
        public int LocationId { get; set; }

        public Location? Location { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<RoomImage> RoomImages { get; set; } = new List<RoomImage>();
        public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
