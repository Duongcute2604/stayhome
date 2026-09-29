using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.DTOs;
using server.Models.Entities;

namespace server.Services
{
    // ============================================================================
    // AMENITY SERVICE - Logic quản lý tiện nghi (chỉ Admin gọi qua Controller)
    // ============================================================================
    public class AmenityService : IAmenityService
    {
        private readonly StayEasyDbContext _context;

        public AmenityService(StayEasyDbContext context)
        {
            _context = context;
        }

        public async Task<List<AmenityDto>> GetAllAsync()
        {
            return await _context.Amenities
                .Select(a => new AmenityDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Icon = a.Icon,
                    Category = a.Category,
                    RoomCount = a.RoomAmenities.Count
                })
                .ToListAsync();
        }

        public async Task<AmenityDto?> GetByIdAsync(int id)
        {
            return await _context.Amenities
                .Where(a => a.Id == id)
                .Select(a => new AmenityDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Icon = a.Icon,
                    Category = a.Category,
                    RoomCount = a.RoomAmenities.Count
                })
                .FirstOrDefaultAsync();
        }

        public async Task<AmenityDto> CreateAsync(CreateAmenityRequest request)
        {
            if (await _context.Amenities.AnyAsync(a => a.Name == request.Name))
                throw new Exception("Tên tiện nghi đã tồn tại");

            var amenity = new Amenity
            {
                Name = request.Name,
                Description = request.Description,
                Icon = request.Icon,
                Category = request.Category
            };

            _context.Amenities.Add(amenity);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(amenity.Id) ?? throw new Exception("Không thể tạo tiện nghi");
        }

        public async Task<AmenityDto?> UpdateAsync(int id, UpdateAmenityRequest request)
        {
            var amenity = await _context.Amenities.FindAsync(id);
            if (amenity == null) return null;

            if (request.Name != null && request.Name != amenity.Name)
            {
                if (await _context.Amenities.AnyAsync(a => a.Name == request.Name))
                    throw new Exception("Tên tiện nghi đã tồn tại");
                amenity.Name = request.Name;
            }

            if (request.Description != null) amenity.Description = request.Description;
            if (request.Icon != null) amenity.Icon = request.Icon;
            if (request.Category != null) amenity.Category = request.Category;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var amenity = await _context.Amenities.FindAsync(id);
            if (amenity == null) return false;

            // Chặn xóa khi đang gán cho phòng (FK Restrict cũng chặn ở DB)
            if (await _context.RoomAmenities.AnyAsync(ra => ra.AmenityId == id))
                throw new Exception("Không thể xóa tiện nghi đang được sử dụng");

            _context.Amenities.Remove(amenity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
