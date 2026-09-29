using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.Common;
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
        // TÌM KIẾM & LỌC PHÒNG (có phân trang)
        // ============================================================================
        // Tại sao dùng IQueryable: Chỉ build 1 query SQL duy nhất (WHERE + ORDER + LIMIT)
        // ============================================================================
        public async Task<PagedResult<RoomDto>> SearchAsync(RoomSearchRequest request)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 12 : Math.Min(request.PageSize, 50);

            var query = _context.Rooms
                .Include(r => r.Location)
                .Include(r => r.RoomImages)
                .Include(r => r.RoomAmenities)
                    .ThenInclude(ra => ra.Amenity)
                .AsQueryable();

            // Từ khóa: tìm trong tên + mô tả
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var keyword = request.Search.Trim();
                query = query.Where(r => r.Name.Contains(keyword) ||
                    (r.Description != null && r.Description.Contains(keyword)));
            }

            if (request.LocationId.HasValue)
                query = query.Where(r => r.LocationId == request.LocationId.Value);

            if (request.MinPrice.HasValue)
                query = query.Where(r => r.PricePerDay >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(r => r.PricePerDay <= request.MaxPrice.Value);

            if (request.Capacity.HasValue)
                query = query.Where(r => r.Capacity >= request.Capacity.Value);

            // Phòng phải có TẤT CẢ tiện nghi được chọn
            if (request.AmenityIds != null && request.AmenityIds.Count > 0)
            {
                var ids = request.AmenityIds;
                query = query.Where(r =>
                    r.RoomAmenities.Count(ra => ids.Contains(ra.AmenityId)) == ids.Count);
            }

            // Chỉ lấy phòng trống trong khoảng check-in/check-out
            if (request.CheckIn.HasValue && request.CheckOut.HasValue &&
                request.CheckOut > request.CheckIn)
            {
                var ci = request.CheckIn.Value;
                var co = request.CheckOut.Value;
                query = query.Where(r => !_context.Bookings.Any(b =>
                    b.RoomId == r.Id &&
                    b.Status != "CANCELLED" &&
                    b.CheckIn < co &&
                    b.CheckOut > ci));
            }

            // Sắp xếp
            query = request.SortBy switch
            {
                "price_asc" => query.OrderBy(r => r.PricePerDay),
                "price_desc" => query.OrderByDescending(r => r.PricePerDay),
                _ => query.OrderByDescending(r => r.Id), // newest
            };

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<RoomDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
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
