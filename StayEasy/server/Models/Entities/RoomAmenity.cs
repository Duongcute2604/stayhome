namespace server.Models.Entities
{
    // ============================================================================
    // ROOM-AMENITY ENTITY - Bảng liên kết nhiều-nhiều giữa Phòng và Tiện nghi
    // ============================================================================
    public class RoomAmenity
    {
        public int RoomId { get; set; }
        public Room? Room { get; set; }

        public int AmenityId { get; set; }
        public Amenity? Amenity { get; set; }
    }
}
