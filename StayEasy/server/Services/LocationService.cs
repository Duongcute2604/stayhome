using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.DTOs;
using server.Models.Entities;

namespace server.Services
{
    // ============================================================================
    // LOCATION SERVICE - Logic quản lý địa điểm (chỉ Admin gọi qua Controller)
    // ============================================================================
    public class LocationService : ILocationService
    {
        private readonly StayEasyDbContext _context;

        public LocationService(StayEasyDbContext context)
        {
            _context = context;
        }

        public async Task<List<LocationDto>> GetAllAsync()
        {
            // RoomCount tính trực tiếp bằng Count, không lưu cột dư thừa
            return await _context.Locations
                .Select(l => new LocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Address = l.Address,
                    Description = l.Description,
                    ImageUrl = l.ImageUrl,
                    RoomCount = _context.Rooms.Count(r => r.LocationId == l.Id)
                })
                .ToListAsync();
        }

        public async Task<LocationDto?> GetByIdAsync(int id)
        {
            return await _context.Locations
                .Where(l => l.Id == id)
                .Select(l => new LocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Address = l.Address,
                    Description = l.Description,
                    ImageUrl = l.ImageUrl,
                    RoomCount = _context.Rooms.Count(r => r.LocationId == l.Id)
                })
                .FirstOrDefaultAsync();
        }

        public async Task<LocationDto> CreateAsync(CreateLocationRequest request)
        {
            // Không cho trùng tên
            if (await _context.Locations.AnyAsync(l => l.Name == request.Name))
                throw new Exception("Tên địa điểm đã tồn tại");

            var location = new Location
            {
                Name = request.Name,
                Address = request.Address,
                Description = request.Description,
                ImageUrl = request.ImageUrl
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(location.Id) ?? throw new Exception("Không thể tạo địa điểm");
        }

        public async Task<LocationDto?> UpdateAsync(int id, UpdateLocationRequest request)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null) return null;

            if (request.Name != null && request.Name != location.Name)
            {
                if (await _context.Locations.AnyAsync(l => l.Name == request.Name))
                    throw new Exception("Tên địa điểm đã tồn tại");
                location.Name = request.Name;
            }

            if (request.Address != null) location.Address = request.Address;
            if (request.Description != null) location.Description = request.Description;
            if (request.ImageUrl != null) location.ImageUrl = request.ImageUrl;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null) return false;

            // Chặn xóa khi còn phòng (FK Restrict cũng chặn ở DB, báo lỗi thân thiện ở đây)
            if (await _context.Rooms.AnyAsync(r => r.LocationId == id))
                throw new Exception("Không thể xóa địa điểm còn phòng");

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
