using System.ComponentModel.DataAnnotations;

namespace server.Models.DTOs
{
    // ============================================================================
    // AMENITY DTOs - Data Transfer Objects cho tiện nghi
    // ============================================================================

    public class AmenityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Category { get; set; }
        public int RoomCount { get; set; }
    }

    public class CreateAmenityRequest
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Icon { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }
    }

    public class UpdateAmenityRequest
    {
        [MaxLength(255)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Icon { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }
    }
}
