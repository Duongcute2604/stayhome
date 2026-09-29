using server.Models.Common;
using server.Models.DTOs;

namespace server.Services
{
    // ============================================================================
    // ROOM SERVICE INTERFACE - Định nghĩa các phương thức quản lý phòng
    // ============================================================================
    public interface IRoomService
    {
        Task<List<RoomDto>> GetAllAsync();
        Task<RoomDto?> GetByIdAsync(int id);
        Task<RoomDto> CreateAsync(CreateRoomRequest request);
        Task<RoomDto?> UpdateAsync(int id, UpdateRoomRequest request);
        Task<bool> DeleteAsync(int id);
        Task<bool> CheckAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut);
        Task<PagedResult<RoomDto>> SearchAsync(RoomSearchRequest request);
    }
}
