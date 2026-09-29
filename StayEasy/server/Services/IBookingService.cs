using server.Models.DTOs;

namespace server.Services
{
    // ============================================================================
    // BOOKING SERVICE INTERFACE - Định nghĩa các phương thức quản lý đặt phòng
    // ============================================================================
    public interface IBookingService
    {
        Task<List<BookingDto>> GetAllAsync();
        Task<List<BookingDto>> GetMyBookingsAsync(int userId);
        Task<BookingDto?> GetByIdAsync(int id);
        Task<BookingDto> CreateAsync(int userId, CreateBookingRequest request);
        Task<BookingDto?> UpdateStatusAsync(int id, UpdateBookingStatusRequest request);
        Task<bool> CancelAsync(int id, int userId);
    }
}
