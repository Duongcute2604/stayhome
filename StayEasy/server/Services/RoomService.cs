using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.DTOs;
using server.Models.Entities;

namespace server.Services
{
    // ============================================================================
    // ROOM SERVICE - Xử lý logic quản lý phòng
    // ============================================================================
    public class RoomService : IRoomService
    {
        private readonly StayEasyDbContext _context;

        public RoomService(StayEasyDbContext context)
        {
            _context = context;
        }

        // ============================================================================
        // LẤY TẤT CẢ PHÒNG
        // ============================================================================
        public async Task<List<RoomDto>> GetAllAsync()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Location)
                .Include(r => r.RoomImages)
                .Include(r => r.RoomAmenities)
                    .ThenInclude(ra => ra.Amenity)
                .ToListAsync();

            return rooms.Select(MapToDto).ToList();
        }

        // ============================================================================
        // LẤY PHÒNG THEO ID
        // ============================================================================
        public async Task<RoomDto?> GetByIdAsync(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Location)
                .Include(r => r.RoomImages)
                .Include(r => r.RoomAmenities)
                    .ThenInclude(ra => ra.Amenity)
                .FirstOrDefaultAsync(r => r.Id == id);

            return room == null ? null : MapToDto(room);
        }

        // ============================================================================
        // TẠO PHÒNG MỚI
        // ============================================================================
        public async Task<RoomDto> CreateAsync(CreateRoomRequest request)
        {
            var room = new Room
            {
                LocationId = request.LocationId,
                Name = request.Name,
                Description = request.Description,
                PricePerHour = request.PricePerHour,
                PricePerDay = request.PricePerDay,
                Capacity = request.Capacity,
                Status = "AVAILABLE"
            };

            // Thêm hình ảnh
            if (request.Images != null)
            {
                room.RoomImages = request.Images.Select(url => new RoomImage { ImageUrl = url }).ToList();
            }

            // Thêm tiện nghi
            if (request.AmenityIds != null)
            {
                room.RoomAmenities = request.AmenityIds.Select(id => new RoomAmenity { AmenityId = id }).ToList();
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(room.Id) ?? throw new Exception("Không thể tạo phòng");
        }

        // ============================================================================
        // CẬP NHẬT PHÒNG
        // ============================================================================
        public async Task<RoomDto?> UpdateAsync(int id, UpdateRoomRequest request)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return null;

            if (request.Name != null) room.Name = request.Name;
            if (request.Description != null) room.Description = request.Description;
            if (request.PricePerHour.HasValue) room.PricePerHour = request.PricePerHour.Value;
            if (request.PricePerDay.HasValue) room.PricePerDay = request.PricePerDay.Value;
            if (request.Capacity.HasValue) room.Capacity = request.Capacity.Value;
            if (request.Status != null) room.Status = request.Status;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        // ============================================================================
        // XÓA PHÒNG
        // ============================================================================
        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return false;

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================================================================
        // KIỂM TRA PHÒNG TRỐNG
        // ============================================================================
        public async Task<bool> CheckAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            // Kiểm tra xem có booking nào trùng thời gian không
            var hasConflict = await _context.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                b.Status != "CANCELLED" &&
                b.CheckIn < checkOut &&
                b.CheckOut > checkIn);

            return !hasConflict;
        }

        // ============================================================================
        // HELPER: Map entity sang DTO
        // ============================================================================
        private static RoomDto MapToDto(Room room)
        {
            return new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Description = room.Description,
                PricePerHour = room.PricePerHour,
                PricePerDay = room.PricePerDay,
                Capacity = room.Capacity,
                Status = room.Status,
                LocationId = room.LocationId,
                LocationName = room.Location?.Name,
                Images = room.RoomImages.Select(i => i.ImageUrl).ToList(),
                Amenities = room.RoomAmenities.Select(ra => new AmenityDto
                {
                    Id = ra.Amenity!.Id,
                    Name = ra.Amenity.Name,
                    Description = ra.Amenity.Description
                }).ToList(),
                CreatedAt = room.CreatedAt
            };
        }
    }
}
